using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Models.User
{
    public class UserEditModel
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        /// <summary>Leave blank to keep the current password.</summary>
        [StringLength(200)]
        public string? NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
