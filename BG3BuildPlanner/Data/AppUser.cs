using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BG3BuildPlanner.Data
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [NotMapped]
        public string Username
        {
            get => UserName ?? string.Empty;
            set => UserName = value;
        }

        // Navigation property
        public virtual ICollection<Build> Builds { get; set; } = new List<Build>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}

