using BG3BuildPlanner.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Data
{
	public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public override DbSet<AppUser> Users { get; set; }
		public DbSet<Character> Characters { get; set; }
		public DbSet<Build> Builds { get; set; }
		public DbSet<Skill> Skills { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Rating> Ratings { get; set; }
		public DbSet<AbilityScore> AbilityScores { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<AppUser>(entity =>
			{
				entity.ToTable("Users");
				entity.Property(user => user.UserName).HasColumnName("Username");
			});
		}
	}
}

