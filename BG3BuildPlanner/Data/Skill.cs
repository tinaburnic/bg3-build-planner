using System.Collections.Generic;

namespace BG3BuildPlanner.Data
{
	public class Skill
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int RequiredLevel { get; set; }

		public virtual ICollection<Build> Builds { get; set; } = new List<Build>();
	}
}

