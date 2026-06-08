using System;
using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
using BG3BuildPlanner.Models.Skill;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BG3BuildPlanner.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SkillController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public SkillController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var skills = _dbContext.Skills
                .Active()
                .OrderBy(s => s.Name)
                .ToList();

            return View(skills);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Search(int? id, string? term)
        {
            var query = _dbContext.Skills.Active();

            if (id.HasValue)
            {
                query = query.Where(s => s.Id == id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(s => EF.Functions.Like(s.Name, pattern));
            }

            var results = query
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.RequiredLevel
                })
                .ToList();

            return Json(results);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Autocomplete(string? term)
        {
            var query = _dbContext.Skills.Active();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(s => EF.Functions.Like(s.Name, pattern));
            }

            var results = query
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    Id = s.Id,
                    Text = s.Name
                })
                .Take(10)
                .ToList();

            return Json(results);
        }

        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skill = _dbContext.Skills
                .Active()
                .Include(s => s.Builds)
                .FirstOrDefault(s => s.Id == id.Value);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new SkillCreateModel { RequiredLevel = 1 });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SkillCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var skill = new Skill
            {
                Name = model.Name,
                Description = model.Description,
                RequiredLevel = model.RequiredLevel,
                ImageUrl = model.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Skills.Add(skill);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = skill.Id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skill = _dbContext.Skills
                .Active()
                .FirstOrDefault(s => s.Id == id.Value);
            if (skill == null)
            {
                return NotFound();
            }

            var model = new SkillEditModel
            {
                Id = skill.Id,
                Name = skill.Name,
                Description = skill.Description,
                RequiredLevel = skill.RequiredLevel,
                ImageUrl = skill.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var skill = await _dbContext.Skills
                .Active()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (skill == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(skill, "",
                s => s.Name,
                s => s.Description,
                s => s.RequiredLevel,
                s => s.ImageUrl))
            {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = skill.Id });
            }

            var model = new SkillEditModel
            {
                Id = skill.Id,
                Name = skill.Name,
                Description = skill.Description,
                RequiredLevel = skill.RequiredLevel,
                ImageUrl = skill.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _dbContext.Skills
                .Active()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (skill == null)
            {
                return NotFound();
            }

            skill.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
