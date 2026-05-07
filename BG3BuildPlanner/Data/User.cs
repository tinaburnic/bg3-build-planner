using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Data
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string PasswordHash { get; set; }

        // Navigation property
        public virtual ICollection<Build> Builds { get; set; } = new List<Build>();
    }
}

