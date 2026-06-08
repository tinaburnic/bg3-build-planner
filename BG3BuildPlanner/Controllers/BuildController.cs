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
        public IActionResult Create()
        {
            return View(new BuildCreateModel());
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin,Builder")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BuildCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var character = _dbContext.Characters
                .FirstOrDefault(c => c.Id == model.CharacterId && c.DeletedAt == null);
            if (character == null)
            {
                ModelState.AddModelError(nameof(model.CharacterId), "Character not found.");
                return View(model);
            }

            var user = _dbContext.Users
                .Active()
                .FirstOrDefault(u => u.Id == model.UserId);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.UserId), "User not found.");
                return View(model);
            }

            var build = new Build
            {
                Title = model.Title,
                Description = model.Description,
                Difficulty = model.Difficulty,
                CharacterId = model.CharacterId,
                UserId = model.UserId,
                CreatedAt = DateTime.UtcNow,
                Character = character,
                User = user
            };

            _dbContext.Builds.Add(build);
            _dbContext.SaveChanges();

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
                UserId = build.UserId
            };

            ViewData["CharacterName"] = await _dbContext.Characters
                .Where(c => c.Id == build.CharacterId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
            ViewData["UserName"] = await _dbContext.Users
                .Active()
                .Where(u => u.Id == build.UserId)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpPost("edit/{id:int}")]
        [Authorize(Roles = "Admin,Builder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
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

            if (await TryUpdateModelAsync(build, "",
                b => b.Title,
                b => b.Description,
                b => b.Difficulty,
                b => b.CharacterId,
                b => b.UserId))
            {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = build.Id });
            }

            var model = new BuildEditModel
            {
                Id = build.Id,
                Title = build.Title,
                Description = build.Description,
                Difficulty = build.Difficulty,
                CharacterId = build.CharacterId,
                UserId = build.UserId
            };

            ViewData["CharacterName"] = await _dbContext.Characters
                .Where(c => c.Id == build.CharacterId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
            ViewData["UserName"] = await _dbContext.Users
                .Where(u => u.Id == build.UserId)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();

            return View(model);
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
