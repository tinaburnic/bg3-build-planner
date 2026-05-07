using System.Linq;
using BG3BuildPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BG3BuildPlanner.Controllers
{
    public class CharacterController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CharacterController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var characters = _dbContext.Characters
                .OrderBy(c => c.Name)
                .ToList();

            return View(characters);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = _dbContext.Characters
                .Include(c => c.Builds)
                .FirstOrDefault(c => c.Id == id.Value);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }
    }
}
