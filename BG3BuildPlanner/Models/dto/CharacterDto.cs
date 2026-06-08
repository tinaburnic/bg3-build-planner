using System;

namespace BG3BuildPlanner.Models.Dto
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PortraitUrl { get; set; }
        public string Race { get; set; } = string.Empty;
        public string Background { get; set; } = string.Empty;
        public int Level { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}