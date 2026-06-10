using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Models.Dto
{
    public class RatingUpdateDto
    {
        [Range(1, 5)]
        public int Score { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int BuildId { get; set; }
    }
}