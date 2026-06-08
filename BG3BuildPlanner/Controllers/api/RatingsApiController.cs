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
    [Route("api/ratings")]
    public class RatingsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public RatingsApiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatings()
        {
            var ratings = await _dbContext.Ratings
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    Score = r.Score,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    BuildId = r.BuildId,
                    UserId = r.UserId
                })
                .ToListAsync();

            return Ok(ratings);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<RatingDto>>> SearchRatings([FromQuery] int? id, [FromQuery] int? buildId, [FromQuery] int? userId, [FromQuery] string? term)
        {
            var query = _dbContext.Ratings
                .AsNoTracking()
                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(r => r.Id == id.Value);
            }

            if (buildId.HasValue)
            {
                query = query.Where(r => r.BuildId == buildId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(r => r.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(r => EF.Functions.Like(r.Comment, pattern));
            }

            var ratings = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    Score = r.Score,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    BuildId = r.BuildId,
                    UserId = r.UserId
                })
                .ToListAsync();

            return Ok(ratings);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RatingDto>> GetRatingById(int id)
        {
            var rating = await _dbContext.Ratings
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    Score = r.Score,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    BuildId = r.BuildId,
                    UserId = r.UserId
                })
                .FirstOrDefaultAsync();

            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        [HttpPost]
        public async Task<ActionResult<RatingDto>> CreateRating([FromBody] RatingCreateDto dto)
        {
            var buildExists = await _dbContext.Builds
                .Active()
                .AnyAsync(b => b.Id == dto.BuildId);
            if (!buildExists)
            {
                ModelState.AddModelError(nameof(dto.BuildId), "Build not found.");
                return ValidationProblem(ModelState);
            }

            var userExists = await _dbContext.Users
                .Active()
                .AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                ModelState.AddModelError(nameof(dto.UserId), "User not found.");
                return ValidationProblem(ModelState);
            }

            var rating = new Rating
            {
                Score = dto.Score,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow,
                BuildId = dto.BuildId,
                UserId = dto.UserId,
                Build = null!,
                User = null!
            };

            _dbContext.Ratings.Add(rating);
            await _dbContext.SaveChangesAsync();

            var result = new RatingDto
            {
                Id = rating.Id,
                Score = rating.Score,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt,
                BuildId = rating.BuildId,
                UserId = rating.UserId
            };

            return CreatedAtAction(nameof(GetRatingById), new { id = rating.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] RatingUpdateDto dto)
        {
            var rating = await _dbContext.Ratings.FirstOrDefaultAsync(r => r.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            var buildExists = await _dbContext.Builds
                .Active()
                .AnyAsync(b => b.Id == dto.BuildId);
            if (!buildExists)
            {
                ModelState.AddModelError(nameof(dto.BuildId), "Build not found.");
                return ValidationProblem(ModelState);
            }

            var userExists = await _dbContext.Users
                .Active()
                .AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                ModelState.AddModelError(nameof(dto.UserId), "User not found.");
                return ValidationProblem(ModelState);
            }

            rating.Score = dto.Score;
            rating.Comment = dto.Comment;
            rating.BuildId = dto.BuildId;
            rating.UserId = dto.UserId;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _dbContext.Ratings.FirstOrDefaultAsync(r => r.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            _dbContext.Ratings.Remove(rating);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}