using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BG3BuildPlanner.Data
{
	public class Build
	{
		[Key]
		public int Id { get; set; }
		public required string Title { get; set; }
		public required string Description { get; set; }
		public Difficulty Difficulty { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }

		[ForeignKey(nameof(User))]
		public int UserId { get; set; }
		public required virtual AppUser User { get; set; }

		[ForeignKey(nameof(Character))]
		public int CharacterId { get; set; }
		public required virtual Character Character { get; set; }

		public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
		public virtual ICollection<Item> Items { get; set; } = new List<Item>();
		public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
	}
}

