using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BG3BuildPlanner.Data
{
	public class AbilityScore
	{
		[Key]
		public int Id { get; set; }
		public int Strength { get; set; }
		public int Dexterity { get; set; }
		public int Constitution { get; set; }
		public int Intelligence { get; set; }
		public int Wisdom { get; set; }
		public int Charisma { get; set; }

		[ForeignKey(nameof(Build))]
		public int BuildId { get; set; }
		public required virtual Build Build { get; set; }
	}
}

