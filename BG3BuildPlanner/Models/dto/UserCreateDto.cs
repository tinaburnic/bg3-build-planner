using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Models.Dto
{
    public class UserCreateDto
    {
        [Required]
        [StringLength(120)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;
    }
}