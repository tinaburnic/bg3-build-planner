using System.ComponentModel.DataAnnotations;
using BG3BuildPlanner.Data;

namespace BG3BuildPlanner.Models.Build
{
    public class BuildEditModel
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

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

        public int[] SkillIds { get; set; } = Array.Empty<int>();
        public int[] ItemIds { get; set; } = Array.Empty<int>();
    }
}
