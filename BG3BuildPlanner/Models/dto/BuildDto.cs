using System;
using BG3BuildPlanner.Data;

namespace BG3BuildPlanner.Models.Dto
{
    public class BuildDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Difficulty Difficulty { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CharacterId { get; set; }
        public int UserId { get; set; }
    }
}