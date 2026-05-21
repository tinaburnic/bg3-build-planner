using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Models.Item;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ItemController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var items = _dbContext.Items
                .OrderBy(i => i.Name)
                .ToList();

            return View(items);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = _dbContext.Items
                .Include(i => i.Builds)
                .FirstOrDefault(i => i.Id == id.Value);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ItemCreateModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ItemCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var item = new Item
            {
                Name = model.Name,
                Type = model.Type,
                Rarity = model.Rarity,
                Power = model.Power
            };

            _dbContext.Items.Add(item);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = item.Id });
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = _dbContext.Items.FirstOrDefault(i => i.Id == id.Value);
            if (item == null)
            {
                return NotFound();
            }

            var model = new ItemEditModel
            {
                Id = item.Id,
                Name = item.Name,
                Type = item.Type,
                Rarity = item.Rarity,
                Power = item.Power
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(item, "",
                i => i.Name,
                i => i.Type,
                i => i.Rarity,
                i => i.Power))
            {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = item.Id });
            }

            var model = new ItemEditModel
            {
                Id = item.Id,
                Name = item.Name,
                Type = item.Type,
                Rarity = item.Rarity,
                Power = item.Power
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            _dbContext.Items.Remove(item);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Search(int? id, string? term)
        {
            var query = _dbContext.Items.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(i => i.Id == id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(i => EF.Functions.Like(i.Name, pattern));
            }

            var results = query
                .OrderBy(i => i.Name)
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    Type = i.Type.ToString(),
                    i.Rarity,
                    i.Power
                })
                .ToList();

            return Json(results);
        }

        [HttpGet]
        public IActionResult Autocomplete(string? term)
        {
            var query = _dbContext.Items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(i => EF.Functions.Like(i.Name, pattern));
            }

            var results = query
                .OrderBy(i => i.Name)
                .Select(i => new
                {
                    Id = i.Id,
                    Text = i.Name
                })
                .Take(10)
                .ToList();

            return Json(results);
        }
    }
}
