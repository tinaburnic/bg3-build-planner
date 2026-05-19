using System.ComponentModel.DataAnnotations;
using BG3BuildPlanner.Data;

namespace BG3BuildPlanner.Models.Build
{
    public class BuildCreateModel
    {
        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Difficulty Difficulty { get; set; }

        [Range(1, int.MaxValue)]
        public int CharacterId { get; set; }

        [Range(1, int.MaxValue)]
        public int UserId { get; set; }
    }
}
