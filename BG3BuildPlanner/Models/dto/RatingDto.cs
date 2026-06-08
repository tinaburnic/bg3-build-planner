using System;

namespace BG3BuildPlanner.Models.Dto
{
    public class RatingDto
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int BuildId { get; set; }
        public int UserId { get; set; }
    }
}