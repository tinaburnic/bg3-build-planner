using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Models.Skill
{
    public class SkillCreateModel
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 20)]
        public int RequiredLevel { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
