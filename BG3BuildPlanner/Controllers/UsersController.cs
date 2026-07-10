using System;
using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
using BG3BuildPlanner.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Controllers
{
    [Route("users")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public UsersController(ApplicationDbContext dbContext, IPasswordHasher<AppUser> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var users = _dbContext.Users
                .Active()
                .Include(u => u.Builds)
                .OrderBy(u => u.UserName)
                .ToList();

            return View(users);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _dbContext.Users
                .Active()
                .Include(u => u.Builds)
                    .ThenInclude(b => b.Character)
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
        [AllowAnonymous]
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
                .OrderBy(u => u.UserName)
                .AsEnumerable()
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.ProfileImageUrl,
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new UserCreateModel());
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PasswordHash = string.Empty;
                return View(model);
            }

            var user = new AppUser
            {
                Username = model.Username,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                NormalizedUserName = model.Username.ToUpperInvariant(),
                NormalizedEmail = model.Email.ToUpperInvariant()
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.PasswordHash);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        [HttpGet("edit/{id:int}")]
        [Authorize]
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

            // Check ownership for non-admins
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!User.IsInRole("Admin") && currentUserId != user.Id.ToString())
            {
                return Forbid();
            }

            ViewData["IsOwnProfile"] = currentUserId == user.Id.ToString();

            var model = new UserEditModel
            {
                Id = user.Id,
                Username = user.Username ?? string.Empty,
                Email = user.Email ?? string.Empty
            };

            return View(model);
        }

        [HttpPost("edit/{id:int}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditModel model)
        {
            var user = await _dbContext.Users
                .Active()
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // Check ownership for non-admins
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!User.IsInRole("Admin") && currentUserId != user.Id.ToString())
            {
                return Forbid();
            }

            // Optional password: validate min length only when a value is provided
            if (!string.IsNullOrWhiteSpace(model.NewPassword) && model.NewPassword.Length < 6)
            {
                ModelState.AddModelError(nameof(UserEditModel.NewPassword), "Password must be at least 6 characters.");
            }

            if (!ModelState.IsValid)
            {
                model.Id = user.Id;
                model.Username = user.Username ?? string.Empty;
                model.Email = user.Email ?? string.Empty;
                model.NewPassword = null;
                model.ConfirmPassword = null;
                ViewData["IsOwnProfile"] = currentUserId == user.Id.ToString();
                return View(model);
            }

            user.Username = model.Username;
            user.Email = model.Email;
            user.NormalizedUserName = model.Username.ToUpperInvariant();
            user.NormalizedEmail = model.Email.ToUpperInvariant();

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                user.SecurityStamp = Guid.NewGuid().ToString();
            }

            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        [HttpPost("delete/{id:int}")]
        [Authorize(Roles = "Admin")]
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
        [AllowAnonymous]
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
                .OrderBy(u => u.UserName)
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
