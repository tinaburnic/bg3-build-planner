using System;
using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
using BG3BuildPlanner.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UsersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var users = _dbContext.Users
                .Active()
                .Include(u => u.Builds)
                .OrderBy(u => u.Username)
                .ToList();

            return View(users);
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _dbContext.Users
                .Active()
                .Include(u => u.Builds)
                .Include(u => u.Ratings)
                    .ThenInclude(r => r.Build)
                .FirstOrDefault(u => u.Id == id.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet("search")]
        public IActionResult Search(int? id, string? term)
        {
            var query = _dbContext.Users
                .Active()
                .Include(u => u.Builds)
                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(u => u.Id == id.Value);
            }
            else
            {
                query = query.SearchUsername(term);
            }

            var results = query
                .OrderBy(u => u.Username)
                .AsEnumerable()
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    BuildCount = u.Builds.Count(b => b.DeletedAt == null),
                    Builds = u.Builds
                        .Where(b => b.DeletedAt == null)
                        .OrderBy(b => b.Title)
                        .Select(b => new
                        {
                            b.Id,
                            b.Title
                        })
                        .ToList()
                })
                .ToList();

            return Json(results);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new UserCreateModel());
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = model.PasswordHash,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        [HttpGet("edit/{id:int}")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _dbContext.Users
                .Active()
                .FirstOrDefault(u => u.Id == id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserEditModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

            return View(model);
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _dbContext.Users
                .Active()
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(user, "",
                u => u.Username,
                u => u.Email,
                u => u.PasswordHash))
            {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = user.Id });
            }

            var model = new UserEditModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

            return View(model);
        }

        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _dbContext.Users
                .Active()
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("autocomplete")]
        public IActionResult Autocomplete(string? term)
        {
            var query = _dbContext.Users
                .Active()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.SearchUsername(term);
            }

            var results = query
                .OrderBy(u => u.Username)
                .Select(u => new
                {
                    Id = u.Id,
                    Text = u.Username
                })
                .Take(10)
                .ToList();

            return Json(results);
        }
    }
}
