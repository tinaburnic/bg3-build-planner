using System.Linq;
using BG3BuildPlanner.Data.Mock;
using Microsoft.AspNetCore.Mvc;

namespace BG3BuildPlanner.Controllers
{
    public class CharacterController : Controller
    {
        private readonly CharacterMockRepository _characterRepository;

        public CharacterController(CharacterMockRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        public IActionResult Index()
        {
            var characters = _characterRepository
                .GetAll()
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

            var character = _characterRepository.GetById(id.Value);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }
    }
}
