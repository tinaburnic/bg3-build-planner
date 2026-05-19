using System;
using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Models.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BG3BuildPlanner.Controllers
{
    public class CharacterController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CharacterController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var characters = _dbContext.Characters
                .Where(c => c.DeletedAt == null)
                .OrderBy(c => c.Name)
                .ToList();

            return View(characters);
        }

        [HttpGet]
        public IActionResult Search(string? term)
        {
            var query = _dbContext.Characters
                .Where(c => c.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(c => EF.Functions.Like(c.Name, pattern));
            }

            var results = query
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Race,
                    c.Background,
                    c.Level
                })
                .ToList();

            return Json(results);
        }

        [HttpGet]
        public IActionResult Autocomplete(string? term)
        {
            var query = _dbContext.Characters
                .Where(c => c.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(c => EF.Functions.Like(c.Name, pattern));
            }

            var results = query
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    Id = c.Id,
                    Text = c.Name
                })
                .Take(10)
                .ToList();

            return Json(results);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = _dbContext.Characters
                .Include(c => c.Builds)
                .FirstOrDefault(c => c.Id == id.Value && c.DeletedAt == null);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CharacterCreateModel { Level = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CharacterCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var character = new Character
            {
                Name = model.Name,
                PortraitUrl = model.PortraitUrl,
                Race = model.Race,
                Background = model.Background,
                Level = model.Level,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Characters.Add(character);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = character.Id });
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = _dbContext.Characters
                .FirstOrDefault(c => c.Id == id.Value && c.DeletedAt == null);
            if (character == null)
            {
                return NotFound();
            }

            var model = new CharacterEditModel
            {
                Id = character.Id,
                Name = character.Name,
                PortraitUrl = character.PortraitUrl,
                Race = character.Race,
                Background = character.Background,
                Level = character.Level
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var character = await _dbContext.Characters
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
            if (character == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(character, "",
                c => c.Name,
                c => c.PortraitUrl,
                c => c.Race,
                c => c.Background,
                c => c.Level))
            {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = character.Id });
            }

            var model = new CharacterEditModel
            {
                Id = character.Id,
                Name = character.Name,
                PortraitUrl = character.PortraitUrl,
                Race = character.Race,
                Background = character.Background,
                Level = character.Level
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var character = await _dbContext.Characters
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
            if (character == null)
            {
                return NotFound();
            }

            character.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
