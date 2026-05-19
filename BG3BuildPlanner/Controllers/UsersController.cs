using System.Linq;
using BG3BuildPlanner.Data;
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

        [HttpGet("autocomplete")]
        public IActionResult Autocomplete(string? term)
        {
            var query = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(u => EF.Functions.Like(u.Username, pattern));
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
