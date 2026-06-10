using System;
using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
using BG3BuildPlanner.Models.Build;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BG3BuildPlanner.Controllers
{
    [Route("builds")]
    [Authorize]
    public class BuildController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public BuildController(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var builds = _dbContext.Builds
                .Active()
                .WithDetails()
                .OrderBy(b => b.Title)
                .ToList();

            return View(builds);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult Search(int? id, string? term)
        {
            var buildsQuery = _dbContext.Builds
                .Active()
                .WithDetails();

            if (id.HasValue)
            {
                buildsQuery = buildsQuery.Where(b => b.Id == id.Value);
            }
            else
            {
                buildsQuery = buildsQuery.SearchTitle(term);
            }

            var builds = buildsQuery
                .OrderBy(b => b.Title)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    Difficulty = b.Difficulty.ToString(),
                    CharacterName = b.Character != null ? b.Character.Name : "Unknown",
                    CreatorName = b.User != null ? b.User.Username : "Unknown"
                })
                .ToList();

            return Json(builds);
        }

        [HttpGet("autocomplete")]
        [AllowAnonymous]
        public IActionResult Autocomplete(string? term)
        {
            var results = _dbContext.Builds
                .Active()
                .SearchTitle(term)
                .OrderBy(b => b.Title)
                .Select(b => new
                {
                    Id = b.Id,
                    Text = b.Title
                })
                .Take(10)
                .ToList();

            return Json(results);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var build = _dbContext.Builds
                .Active()
                .WithDetails()
                .FirstOrDefault(b => b.Id == id.Value);
            if (build == null)
            {
                return NotFound();
            }

            return View(build);
        }

        [HttpGet("character/{characterId:int}")]
        [AllowAnonymous]
        public IActionResult ByCharacter(int characterId)
        {
            var builds = _dbContext.Builds
                .Active()
                .WithDetails()
                .ForCharacter(characterId)
                .OrderBy(b => b.Title)
                .ToList();

            return View("Index", builds);
        }

        [HttpGet("top")]
        [AllowAnonymous]
        public IActionResult Top(int take = 3, int minRatings = 1)
        {
            if (take <= 0)
            {
                return BadRequest("Take must be greater than zero.");
            }

            if (minRatings < 0)
            {
                return BadRequest("MinRatings cannot be negative.");
            }

            var builds = _dbContext.Builds
                .Active()
                .WithDetails()
                .TopByAverageRating(take, minRatings)
                .ToList();

            return View("Index", builds);
        }

        [HttpGet("create")]
        [Authorize(Roles = "Admin,Builder")]
        public async Task<IActionResult> Create()
        {
            ViewData["AllSkills"] = await _dbContext.Skills
                .Where(s => s.DeletedAt == null)
                .OrderBy(s => s.Name)
                .ToListAsync();
            ViewData["AllItems"] = await _dbContext.Items
                .OrderBy(i => i.Name)
                .ToListAsync();

            return View(new BuildCreateModel());
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin,Builder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuildCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["AllSkills"] = await _dbContext.Skills
                    .Where(s => s.DeletedAt == null)
                    .OrderBy(s => s.Name)
                    .ToListAsync();
                ViewData["AllItems"] = await _dbContext.Items
                    .OrderBy(i => i.Name)
                    .ToListAsync();
                return View(model);
            }

            var character = await _dbContext.Characters
                .FirstOrDefaultAsync(c => c.Id == model.CharacterId && c.DeletedAt == null);
            if (character == null)
            {
                ModelState.AddModelError(nameof(model.CharacterId), "Character not found.");
                ViewData["AllSkills"] = await _dbContext.Skills
                    .Where(s => s.DeletedAt == null)
                    .OrderBy(s => s.Name)
                    .ToListAsync();
                ViewData["AllItems"] = await _dbContext.Items
                    .OrderBy(i => i.Name)
                    .ToListAsync();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }

            var selectedSkillIds = (model.SkillIds ?? Array.Empty<int>()).Distinct().ToArray();
            var selectedItemIds = (model.ItemIds ?? Array.Empty<int>()).Distinct().ToArray();

            var selectedSkills = await _dbContext.Skills
                .Where(s => s.DeletedAt == null && selectedSkillIds.Contains(s.Id))
                .ToListAsync();
            var selectedItems = await _dbContext.Items
                .Where(i => selectedItemIds.Contains(i.Id))
                .ToListAsync();

            var build = new Build
            {
                Title = model.Title,
                Description = model.Description,
                Difficulty = model.Difficulty,
                CharacterId = model.CharacterId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Character = character,
                User = user
            };

            foreach (var skill in selectedSkills)
            {
                build.Skills.Add(skill);
            }

            foreach (var item in selectedItems)
            {
                build.Items.Add(item);
            }

            _dbContext.Builds.Add(build);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = build.Id });
        }

        [HttpGet("edit/{id:int}")]
        [Authorize(Roles = "Admin,Builder")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var build = _dbContext.Builds
                .Active()
                .WithDetails()
                .FirstOrDefault(b => b.Id == id.Value);
            if (build == null)
            {
                return NotFound();
            }

            // Check ownership for builders
            if (!User.IsInRole("Admin"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Id != build.UserId)
                {
                    return Forbid();
                }
            }

            var model = new BuildEditModel
            {
                Id = build.Id,
                Title = build.Title,
                Description = build.Description,
                Difficulty = build.Difficulty,
                CharacterId = build.CharacterId,
                SkillIds = build.Skills.Select(s => s.Id).ToArray(),
                ItemIds = build.Items.Select(i => i.Id).ToArray()
            };

            ViewData["CharacterName"] = await _dbContext.Characters
                .Where(c => c.Id == build.CharacterId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
            ViewData["AllSkills"] = await _dbContext.Skills
                .Where(s => s.DeletedAt == null)
                .OrderBy(s => s.Name)
                .ToListAsync();
            ViewData["AllItems"] = await _dbContext.Items
                .OrderBy(i => i.Name)
                .ToListAsync();

            return View(model);
        }

        [HttpPost("edit/{id:int}")]
        [Authorize(Roles = "Admin,Builder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BuildEditModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var build = await _dbContext.Builds
                .Active()
                .Include(b => b.Skills)
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (build == null)
            {
                return NotFound();
            }

            // Check ownership for builders
            if (!User.IsInRole("Admin"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Id != build.UserId)
                {
                    return Forbid();
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["CharacterName"] = await _dbContext.Characters
                    .Where(c => c.Id == model.CharacterId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();
                ViewData["AllSkills"] = await _dbContext.Skills
                    .Where(s => s.DeletedAt == null)
                    .OrderBy(s => s.Name)
                    .ToListAsync();
                ViewData["AllItems"] = await _dbContext.Items
                    .OrderBy(i => i.Name)
                    .ToListAsync();
                return View(model);
            }

            var characterExists = await _dbContext.Characters
                .AnyAsync(c => c.Id == model.CharacterId && c.DeletedAt == null);
            if (!characterExists)
            {
                ModelState.AddModelError(nameof(model.CharacterId), "Character not found.");
                ViewData["CharacterName"] = await _dbContext.Characters
                    .Where(c => c.Id == model.CharacterId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();
                ViewData["AllSkills"] = await _dbContext.Skills
                    .Where(s => s.DeletedAt == null)
                    .OrderBy(s => s.Name)
                    .ToListAsync();
                ViewData["AllItems"] = await _dbContext.Items
                    .OrderBy(i => i.Name)
                    .ToListAsync();
                return View(model);
            }

            var selectedSkillIds = (model.SkillIds ?? Array.Empty<int>()).Distinct().ToArray();
            var selectedItemIds = (model.ItemIds ?? Array.Empty<int>()).Distinct().ToArray();

            var selectedSkills = await _dbContext.Skills
                .Where(s => s.DeletedAt == null && selectedSkillIds.Contains(s.Id))
                .ToListAsync();
            var selectedItems = await _dbContext.Items
                .Where(i => selectedItemIds.Contains(i.Id))
                .ToListAsync();

            build.Title = model.Title;
            build.Description = model.Description;
            build.Difficulty = model.Difficulty;
            build.CharacterId = model.CharacterId;

            build.Skills.Clear();
            foreach (var skill in selectedSkills)
            {
                build.Skills.Add(skill);
            }

            build.Items.Clear();
            foreach (var item in selectedItems)
            {
                build.Items.Add(item);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = build.Id });
        }

        [HttpPost("delete/{id:int}")]
        [Authorize(Roles = "Admin,Builder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var build = await _dbContext.Builds
                .Active()
                .FirstOrDefaultAsync(b => b.Id == id);
            if (build == null)
            {
                return NotFound();
            }

            // Check ownership for builders
            if (!User.IsInRole("Admin"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Id != build.UserId)
                {
                    return Forbid();
                }
            }

            build.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
