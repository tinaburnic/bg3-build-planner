using System;
using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
using BG3BuildPlanner.Models.Build;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BG3BuildPlanner.Controllers
{
    [Route("builds")]
    public class BuildController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public BuildController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
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
        public IActionResult Search(string? term)
        {
            var builds = _dbContext.Builds
                .Active()
                .WithDetails()
                .SearchTitle(term)
                .OrderBy(b => b.Title)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    Difficulty = b.Difficulty.ToString(),
                    CharacterName = b.Character != null ? b.Character.Name : "Unknown"
                })
                .ToList();

            return Json(builds);
        }

        [HttpGet("autocomplete")]
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
        public IActionResult Create()
        {
            return View(new BuildCreateModel());
        }

        [HttpPost("create")]
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

        [HttpPost("edit/{id:int}")]
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

            build.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
