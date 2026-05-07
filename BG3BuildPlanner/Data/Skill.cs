using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Data
{
	public class Skill
	{
		[Key]
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
		public int RequiredLevel { get; set; }
		public required string ImageUrl { get; set; } = string.Empty;

		public virtual ICollection<Build> Builds { get; set; } = new List<Build>();
	}
}

