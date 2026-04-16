using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Mock;
using BG3BuildPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace BG3BuildPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BuildMockRepository _buildRepository;

        public HomeController(ILogger<HomeController> logger, BuildMockRepository buildRepository)
        {
            _logger = logger;
            _buildRepository = buildRepository;
        }

        public IActionResult Index()
        {
            var featuredBuilds = _buildRepository
                .GetAll()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

