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
    [Route("api/items")]
    public class ItemsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ItemsApiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            var items = await _dbContext.Items
                .AsNoTracking()
                .OrderBy(i => i.Name)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Type = i.Type,
                    Rarity = i.Rarity,
                    Power = i.Power
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ItemDto>>> SearchItems([FromQuery] int? id, [FromQuery] string? term)
        {
            var query = _dbContext.Items
                .AsNoTracking()
                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(i => i.Id == id.Value);
            }
            else if (!string.IsNullOrWhiteSpace(term))
            {
                var pattern = $"%{term.Trim()}%";
                query = query.Where(i => EF.Functions.Like(i.Name, pattern));
            }

            var items = await query
                .OrderBy(i => i.Name)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Type = i.Type,
                    Rarity = i.Rarity,
                    Power = i.Power
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ItemDto>> GetItemById(int id)
        {
            var item = await _dbContext.Items
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Type = i.Type,
                    Rarity = i.Rarity,
                    Power = i.Power
                })
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItem([FromBody] ItemCreateDto dto)
        {
            var item = new Item
            {
                Name = dto.Name,
                Type = dto.Type,
                Rarity = dto.Rarity,
                Power = dto.Power
            };

            _dbContext.Items.Add(item);
            await _dbContext.SaveChangesAsync();

            var result = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Type = item.Type,
                Rarity = item.Rarity,
                Power = item.Power
            };

            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemUpdateDto dto)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            item.Name = dto.Name;
            item.Type = dto.Type;
            item.Rarity = dto.Rarity;
            item.Power = dto.Power;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            _dbContext.Items.Remove(item);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}