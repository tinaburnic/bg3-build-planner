using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data;
using BG3BuildPlanner.Data.Queries;
using BG3BuildPlanner.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Controllers.Api
{
    [ApiController]
    [Route("api/skills")]
    public class SkillsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public SkillsApiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillDto>>> GetSkills()
        {
            var skills = await _dbContext.Skills
                .AsNoTracking()
                .Active()
                .OrderBy(s => s.Name)
                .Select(s => new SkillDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    RequiredLevel = s.RequiredLevel,
                    ImageUrl = s.ImageUrl,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return Ok(skills);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SkillDto>>> SearchSkills([FromQuery] int? id, [FromQuery] string? term)
        {
            var query = _dbContext.Skills
                .AsNoTracking()
                .Active()
                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(s => s.Id == id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(s => EF.Functions.Like(s.Name, pattern));
            }

            var skills = await query
                .OrderBy(s => s.Name)
                .Select(s => new SkillDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    RequiredLevel = s.RequiredLevel,
                    ImageUrl = s.ImageUrl,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return Ok(skills);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SkillDto>> GetSkillById(int id)
        {
            var skill = await _dbContext.Skills
                .AsNoTracking()
                .Active()
                .Where(s => s.Id == id)
                .Select(s => new SkillDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    RequiredLevel = s.RequiredLevel,
                    ImageUrl = s.ImageUrl,
                    CreatedAt = s.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (skill == null)
            {
                return NotFound();
            }

            return Ok(skill);
        }

        [HttpPost]
        public async Task<ActionResult<SkillDto>> CreateSkill([FromBody] SkillCreateDto dto)
        {
            var skill = new Skill
            {
                Name = dto.Name,
                Description = dto.Description,
                RequiredLevel = dto.RequiredLevel,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Skills.Add(skill);
            await _dbContext.SaveChangesAsync();

            var result = new SkillDto
            {
                Id = skill.Id,
                Name = skill.Name,
                Description = skill.Description,
                RequiredLevel = skill.RequiredLevel,
                ImageUrl = skill.ImageUrl,
                CreatedAt = skill.CreatedAt
            };

            return CreatedAtAction(nameof(GetSkillById), new { id = skill.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] SkillUpdateDto dto)
        {
            var skill = await _dbContext.Skills
                .Active()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (skill == null)
            {
                return NotFound();
            }

            skill.Name = dto.Name;
            skill.Description = dto.Description;
            skill.RequiredLevel = dto.RequiredLevel;
            skill.ImageUrl = dto.ImageUrl;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await _dbContext.Skills
                .Active()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (skill == null)
            {
                return NotFound();
            }

            skill.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}