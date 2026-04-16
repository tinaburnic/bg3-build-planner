using System.Linq;
using BG3BuildPlanner.Data.Mock;
using Microsoft.AspNetCore.Mvc;

namespace BG3BuildPlanner.Controllers
{
    public class SkillController : Controller
    {
        private readonly SkillMockRepository _skillRepository;

        public SkillController(SkillMockRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public IActionResult Index()
        {
            var skills = _skillRepository
                .GetAll()
                .OrderBy(s => s.Name)
                .ToList();

            return View(skills);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skill = _skillRepository.GetById(id.Value);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }
    }
}
