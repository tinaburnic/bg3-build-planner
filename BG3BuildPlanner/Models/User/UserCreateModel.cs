using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Models.User
{
    public class UserCreateModel
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
        public string PasswordHash { get; set; } = string.Empty;
    }
}
