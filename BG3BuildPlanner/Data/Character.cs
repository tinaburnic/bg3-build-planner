using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Data
{
    public class Character
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? PortraitUrl { get; set; }
        public required string Race { get; set; }
        public required string Background { get; set; }
        public int Level { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Build> Builds { get; set; } = new List<Build>();
    }
}

