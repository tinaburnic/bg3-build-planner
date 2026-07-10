using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Models.Character
{
    public class CharacterCreateModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"^(https?://|ftp://|/).+", ErrorMessage = "Portrait URL must be a valid URL or a root-relative path starting with /.")]
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
