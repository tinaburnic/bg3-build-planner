using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
using BG3BuildPlanner.Models;
using BG3BuildPlanner.Models.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace BG3BuildPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var featuredBuilds = _dbContext.Builds
                .Include(b => b.Ratings)
                .OrderByDescending(b => b.Ratings.Any() ? b.Ratings.Average(r => r.Score) : 0)
                .ThenByDescending(b => b.CreatedAt)
                .Take(3)
                .ToList();

            var viewModel = new HomeIndexViewModel
            {
                FeaturedBuilds = featuredBuilds
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Search(string? term)
        {
            var normalizedTerm = term?.Trim() ?? string.Empty;
            var viewModel = new GlobalSearchViewModel
            {
                Term = normalizedTerm
            };

            if (string.IsNullOrWhiteSpace(normalizedTerm))
            {
                return View(viewModel);
            }

            var characters = _dbContext.Characters
                .Active()
                .SearchName(normalizedTerm)
                .OrderBy(c => c.Name)
                .Select(c => new GlobalSearchResultItemViewModel
                {
                    Id = c.Id,
                    Title = c.Name,
                    Subtitle = $"{c.Race} • Level {c.Level}",
                    Description = c.Background,
                    Controller = "Character",
                    TypeLabel = "Character"
                })
                .Take(8)
                .ToList();

            var builds = _dbContext.Builds
                .Active()
                .Include(b => b.Character)
                .SearchTitle(normalizedTerm)
                .OrderBy(b => b.Title)
                .Select(b => new GlobalSearchResultItemViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Subtitle = b.Character != null ? $"{b.Character.Name} • {b.Difficulty}" : b.Difficulty.ToString(),
                    Description = b.Description,
                    Controller = "Build",
                    TypeLabel = "Build"
                })
                .Take(8)
                .ToList();

            var skills = _dbContext.Skills
                .Active()
                .SearchName(normalizedTerm)
                .OrderBy(s => s.Name)
                .Select(s => new GlobalSearchResultItemViewModel
                {
                    Id = s.Id,
                    Title = s.Name,
                    Subtitle = $"Required level {s.RequiredLevel}",
                    Description = s.Description,
                    Controller = "Skill",
                    TypeLabel = "Skill"
                })
                .Take(8)
                .ToList();

            var items = _dbContext.Items
                .SearchName(normalizedTerm)
                .OrderBy(i => i.Name)
                .Select(i => new GlobalSearchResultItemViewModel
                {
                    Id = i.Id,
                    Title = i.Name,
                    Subtitle = $"{i.Type} • {i.Rarity}",
                    Description = $"Power {i.Power}",
                    Controller = "Item",
                    TypeLabel = "Item"
                })
                .Take(8)
                .ToList();

            viewModel = new GlobalSearchViewModel
            {
                Term = normalizedTerm,
                Characters = characters,
                Builds = builds,
                Skills = skills,
                Items = items
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

