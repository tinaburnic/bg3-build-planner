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
    [Route("api/builds")]
    public class BuildsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public BuildsApiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildDto>>> GetBuilds()
        {
            var builds = await _dbContext.Builds
                .AsNoTracking()
                .Active()
                .OrderBy(b => b.Id)
                .Select(b => new BuildDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Difficulty = b.Difficulty,
                    CreatedAt = b.CreatedAt,
                    CharacterId = b.CharacterId,
                    UserId = b.UserId
                })
                .ToListAsync();

            return Ok(builds);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BuildDto>> GetBuildById(int id)
        {
            var build = await _dbContext.Builds
                .AsNoTracking()
                .Active()
                .Where(b => b.Id == id)
                .Select(b => new BuildDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Difficulty = b.Difficulty,
                    CreatedAt = b.CreatedAt,
                    CharacterId = b.CharacterId,
                    UserId = b.UserId
                })
                .FirstOrDefaultAsync();

            if (build == null)
            {
                return NotFound();
            }

            return Ok(build);
        }

        [HttpPost]
        public async Task<ActionResult<BuildDto>> CreateBuild([FromBody] BuildCreateDto dto)
        {
            var characterExists = await _dbContext.Characters
                .AnyAsync(c => c.Id == dto.CharacterId && c.DeletedAt == null);
            if (!characterExists)
            {
                ModelState.AddModelError(nameof(dto.CharacterId), "Character not found.");
                return ValidationProblem(ModelState);
            }

            var userExists = await _dbContext.Users
                .AnyAsync(u => u.Id == dto.UserId && u.DeletedAt == null);
            if (!userExists)
            {
                ModelState.AddModelError(nameof(dto.UserId), "User not found.");
                return ValidationProblem(ModelState);
            }

            var build = new Build
            {
                Title = dto.Title,
                Description = dto.Description,
                Difficulty = dto.Difficulty,
                CharacterId = dto.CharacterId,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow,
                User = null!,
                Character = null!
            };

            _dbContext.Builds.Add(build);
            await _dbContext.SaveChangesAsync();

            var result = new BuildDto
            {
                Id = build.Id,
                Title = build.Title,
                Description = build.Description,
                Difficulty = build.Difficulty,
                CreatedAt = build.CreatedAt,
                CharacterId = build.CharacterId,
                UserId = build.UserId
            };

            return CreatedAtAction(nameof(GetBuildById), new { id = build.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBuild(int id, [FromBody] BuildUpdateDto dto)
        {
            var build = await _dbContext.Builds
                .Active()
                .FirstOrDefaultAsync(b => b.Id == id);
            if (build == null)
            {
                return NotFound();
            }

            var characterExists = await _dbContext.Characters
                .AnyAsync(c => c.Id == dto.CharacterId && c.DeletedAt == null);
            if (!characterExists)
            {
                ModelState.AddModelError(nameof(dto.CharacterId), "Character not found.");
                return ValidationProblem(ModelState);
            }

            var userExists = await _dbContext.Users
                .AnyAsync(u => u.Id == dto.UserId && u.DeletedAt == null);
            if (!userExists)
            {
                ModelState.AddModelError(nameof(dto.UserId), "User not found.");
                return ValidationProblem(ModelState);
            }

            build.Title = dto.Title;
            build.Description = dto.Description;
            build.Difficulty = dto.Difficulty;
            build.CharacterId = dto.CharacterId;
            build.UserId = dto.UserId;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBuild(int id)
        {
            var build = await _dbContext.Builds
                .Active()
                .FirstOrDefaultAsync(b => b.Id == id);
            if (build == null)
            {
                return NotFound();
            }

            build.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}