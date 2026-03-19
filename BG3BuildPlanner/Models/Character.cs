using System;
using System.Collections.Generic;

namespace BG3BuildPlanner.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Race { get; set; }
        public string Background { get; set; }
        public int Level { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Build> Builds { get; set; } = new List<Build>();
    }
}
