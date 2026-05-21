using System;
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
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation property
        public virtual ICollection<Build> Builds { get; set; } = new List<Build>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}

