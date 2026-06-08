using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Controllers.Api
{
    [ApiController]
    [Route("api/characters")]
    public class CharactersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CharactersApiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterDto>>> GetCharacters()
        {
            var characters = await _dbContext.Characters
                .AsNoTracking()
                .Where(c => c.DeletedAt == null)
                .OrderBy(c => c.Name)
                .Select(c => new CharacterDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    PortraitUrl = c.PortraitUrl,
                    Race = c.Race,
                    Background = c.Background,
                    Level = c.Level,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(characters);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CharacterDto>>> SearchCharacters([FromQuery] int? id, [FromQuery] string? term)
        {
            var query = _dbContext.Characters
                .AsNoTracking()
                .Where(c => c.DeletedAt == null)
                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(c => c.Id == id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(c => EF.Functions.Like(c.Name, pattern));
            }

            var characters = await query
                .OrderBy(c => c.Name)
                .Select(c => new CharacterDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    PortraitUrl = c.PortraitUrl,
                    Race = c.Race,
                    Background = c.Background,
                    Level = c.Level,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(characters);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CharacterDto>> GetCharacterById(int id)
        {
            var character = await _dbContext.Characters
                .AsNoTracking()
                .Where(c => c.Id == id && c.DeletedAt == null)
                .Select(c => new CharacterDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    PortraitUrl = c.PortraitUrl,
                    Race = c.Race,
                    Background = c.Background,
                    Level = c.Level,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (character == null)
            {
                return NotFound();
            }

            return Ok(character);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterDto>> CreateCharacter([FromBody] CharacterCreateDto dto)
        {
            var character = new Character
            {
                Name = dto.Name,
                PortraitUrl = dto.PortraitUrl,
                Race = dto.Race,
                Background = dto.Background,
                Level = dto.Level,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Characters.Add(character);
            await _dbContext.SaveChangesAsync();

            var result = new CharacterDto
            {
                Id = character.Id,
                Name = character.Name,
                PortraitUrl = character.PortraitUrl,
                Race = character.Race,
                Background = character.Background,
                Level = character.Level,
                CreatedAt = character.CreatedAt
            };

            return CreatedAtAction(nameof(GetCharacterById), new { id = character.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCharacter(int id, [FromBody] CharacterUpdateDto dto)
        {
            var character = await _dbContext.Characters
                .Where(c => c.Id == id && c.DeletedAt == null)
                .FirstOrDefaultAsync();
            if (character == null)
            {
                return NotFound();
            }

            character.Name = dto.Name;
            character.PortraitUrl = dto.PortraitUrl;
            character.Race = dto.Race;
            character.Background = dto.Background;
            character.Level = dto.Level;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            var character = await _dbContext.Characters
                .Where(c => c.Id == id && c.DeletedAt == null)
                .FirstOrDefaultAsync();
            if (character == null)
            {
                return NotFound();
            }

            character.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}