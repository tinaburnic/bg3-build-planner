using System;
using System.Collections.Generic;

namespace BG3BuildPlanner.Data
{
	public class Build
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Difficulty Difficulty { get; set; }
		public DateTime CreatedAt { get; set; }

		public int CharacterId { get; set; }
		public virtual Character Character { get; set; }

		public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
		public virtual ICollection<Item> Items { get; set; } = new List<Item>();
		public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
	}
}

