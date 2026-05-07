using System.Linq;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
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
                .WithDetails()
                .OrderBy(b => b.Title)
                .ToList();

            return View(builds);
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var build = _dbContext.Builds
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
                .WithDetails()
                .TopByAverageRating(take, minRatings)
                .ToList();

            return View("Index", builds);
        }
    }
}
