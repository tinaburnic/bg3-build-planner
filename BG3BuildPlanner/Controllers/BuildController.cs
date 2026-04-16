using System.Linq;
using BG3BuildPlanner.Data.Mock;
using Microsoft.AspNetCore.Mvc;

namespace BG3BuildPlanner.Controllers
{
    public class BuildController : Controller
    {
        private readonly BuildMockRepository _buildRepository;
        private readonly CharacterMockRepository _characterRepository;

        public BuildController(BuildMockRepository buildRepository, CharacterMockRepository characterRepository)
        {
            _buildRepository = buildRepository;
            _characterRepository = characterRepository;
        }

        public IActionResult Index()
        {
            var builds = _buildRepository
                .GetAll()
                .OrderBy(b => b.Title)
                .ToList();

            return View(builds);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var build = _buildRepository.GetById(id.Value);
            if (build == null)
            {
                return NotFound();
            }

            var characterName = _characterRepository.GetById(build.CharacterId)?.Name ?? "Unknown";
            ViewData["CharacterName"] = characterName;

            return View(build);
        }
    }
}
