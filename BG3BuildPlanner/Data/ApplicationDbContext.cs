using BG3BuildPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Character> Characters { get; set; }
		public DbSet<Build> Builds { get; set; }
		public DbSet<Skill> Skills { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Rating> Ratings { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<AbilityScore> AbilityScores { get; set; }
	}
}
