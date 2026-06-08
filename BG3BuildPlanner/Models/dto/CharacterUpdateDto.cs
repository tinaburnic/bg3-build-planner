using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Models.Dto
{
    public class CharacterUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Url]
        public string? PortraitUrl { get; set; }

        [Required]
        [StringLength(100)]
        public string Race { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Background { get; set; } = string.Empty;

        [Range(1, 12)]
        public int Level { get; set; }
    }
}