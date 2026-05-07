using System.Linq;
using BG3BuildPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BG3BuildPlanner.Controllers
{
    public class SkillController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public SkillController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var skills = _dbContext.Skills
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

            var skill = _dbContext.Skills
                .Include(s => s.Builds)
                .FirstOrDefault(s => s.Id == id.Value);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }
    }
}
