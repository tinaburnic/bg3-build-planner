using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BG3BuildPlanner.Data.Mock;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Data
{
	public static class DbInitializer
	{
		public static async Task InitializeAsync(ApplicationDbContext context, RoleManager<IdentityRole<int>> roleManager, UserManager<AppUser> userManager)
		{
			if (context.Database.IsRelational())
			{
				await context.Database.MigrateAsync();
			}
			else
			{
				await context.Database.EnsureCreatedAsync();
			}

			await SeedRoles(roleManager);

			var passwordHasher = new PasswordHasher<AppUser>();
			var userSeeds = new (string Username, string Email, string Password)[]
			{
				("demo", "demo@example.com", "demo"),
				("aria", "aria@baldurs-gate.com", "aria"),
				("bren", "bren@baldurs-gate.com", "bren"),
				("kestrel", "kestrel@baldurs-gate.com", "kestrel"),
				("lyra", "lyra@baldurs-gate.com", "lyra")
			};

			if (context.Characters.Any())
			{
				await EnsureSeedUsersAsync(context, passwordHasher, userManager, userSeeds);
				return;
			}

			var users = new List<AppUser>
			{
				CreateUser(passwordHasher, "demo", "demo@example.com", "demo"),
				CreateUser(passwordHasher, "aria", "aria@baldurs-gate.com", "aria"),
				CreateUser(passwordHasher, "bren", "bren@baldurs-gate.com", "bren"),
				CreateUser(passwordHasher, "kestrel", "kestrel@baldurs-gate.com", "kestrel"),
				CreateUser(passwordHasher, "lyra", "lyra@baldurs-gate.com", "lyra")
			};

			var buildOwners = users.Take(Math.Max(1, users.Count - 1)).ToList();

			var characterRepository = new CharacterMockRepository();
			var buildRepository = new BuildMockRepository();
			var skillRepository = new SkillMockRepository();

			var characters = characterRepository.GetAll()
				.Select(c => new Character
				{
					Id = c.Id,
					Name = c.Name,
					PortraitUrl = c.PortraitUrl,
					Race = c.Race,
					Background = c.Background,
					Level = c.Level,
					CreatedAt = c.CreatedAt
				})
				.ToList();

			var charactersById = characters.ToDictionary(c => c.Id);
			var skillsByName = skillRepository.GetAll()
				.GroupBy(s => s.Name)
				.ToDictionary(
					g => g.Key,
					g => new Skill
					{
						Name = g.First().Name,
						Description = g.First().Description,
						RequiredLevel = g.First().RequiredLevel,
						ImageUrl = g.First().ImageUrl
					});
			var itemsByKey = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);

			var builds = buildRepository.GetAll()
				.Select((b, index) =>
				{
					var owner = buildOwners[index % buildOwners.Count];
					var eligibleRaters = users.Where(u => u != owner).ToList();
					var ratingIndex = 0;
					var build = new Build
					{
						Title = b.Title,
						Description = b.Description,
						Difficulty = b.Difficulty,
						CreatedAt = b.CreatedAt,
						Character = charactersById[b.CharacterId],
						User = owner,
						Skills = b.Skills.Select(s => GetOrCreateSkill(skillsByName, s)).ToList(),
						Items = b.Items.Select(i => GetOrCreateItem(itemsByKey, i)).ToList(),
						Ratings = new List<Rating>()
					};

					foreach (var rating in b.Ratings)
					{
						if (eligibleRaters.Count == 0) break;

						var rater = eligibleRaters[ratingIndex % eligibleRaters.Count];
						ratingIndex++;
						build.Ratings.Add(new Rating
						{
							Score = rating.Score,
							Comment = rating.Comment,
							CreatedAt = rating.CreatedAt,
							Build = build,
							User = rater
						});
					}

					return build;
				})
				.ToList();

			context.Users.AddRange(users);
			context.Characters.AddRange(characters);
			context.Skills.AddRange(skillsByName.Values);
			context.Items.AddRange(itemsByKey.Values);
			context.Builds.AddRange(builds);
			await context.SaveChangesAsync();

			await AssignRolesToUsers(userManager, users);
        }

		private static async Task SeedRoles(RoleManager<IdentityRole<int>> roleManager)
		{
			foreach (var role in new[] { "Admin", "Builder" })
			{
				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
				}
			}
		}

		private static async Task AssignRolesToUsers(UserManager<AppUser> userManager, List<AppUser> users)
		{
			var demoUser = users.FirstOrDefault(u => u.UserName == "demo");
			if (demoUser != null && !await userManager.IsInRoleAsync(demoUser, "Admin"))
			{
				await userManager.AddToRoleAsync(demoUser, "Admin");
			}

			foreach (var user in users.Where(u => u.UserName != "demo"))
			{
				if (!await userManager.IsInRoleAsync(user, "Builder"))
				{
					await userManager.AddToRoleAsync(user, "Builder");
				}
			}
		}

		private static Skill GetOrCreateSkill(IDictionary<string, Skill> skillsByName, Skill source)
		{
			if (skillsByName.TryGetValue(source.Name, out var existing)) return existing;
			var created = new Skill { Name = source.Name, Description = source.Description, RequiredLevel = source.RequiredLevel, ImageUrl = source.ImageUrl };
			skillsByName[source.Name] = created;
			return created;
		}

		private static Item GetOrCreateItem(IDictionary<string, Item> itemsByKey, Item source)
		{
			var key = $"{source.Name}|{source.Type}|{source.Rarity}|{source.Power}";
			if (itemsByKey.TryGetValue(key, out var existing)) return existing;
			var created = new Item { Name = source.Name, Type = source.Type, Rarity = source.Rarity, Power = source.Power };
			itemsByKey[key] = created;
			return created;
		}

		private static AppUser CreateUser(IPasswordHasher<AppUser> passwordHasher, string username, string email, string password)
		{
			var user = new AppUser
			{
				Username = username,
				Email = email,
				CreatedAt = DateTime.UtcNow,
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				SecurityStamp = Guid.NewGuid().ToString(),
				NormalizedUserName = username.ToUpperInvariant(),
				NormalizedEmail = email.ToUpperInvariant()
			};
			user.PasswordHash = passwordHasher.HashPassword(user, password);
			return user;
		}

		private static async Task EnsureSeedUsersAsync(
			ApplicationDbContext context,
			IPasswordHasher<AppUser> passwordHasher,
			UserManager<AppUser> userManager,
			IEnumerable<(string Username, string Email, string Password)> userSeeds)
		{
			var updated = false;
			var users = new List<AppUser>();

			foreach (var seed in userSeeds)
			{
				var user = context.Users.FirstOrDefault(u => u.UserName == seed.Username || u.Email == seed.Email);
				if (user == null)
				{
					user = CreateUser(passwordHasher, seed.Username, seed.Email, seed.Password);
					context.Users.Add(user);
					updated = true;
				}
				else
				{
					user.Username = seed.Username;
					user.Email = seed.Email;
					user.CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt;
					user.NormalizedUserName = seed.Username.ToUpperInvariant();
					user.NormalizedEmail = seed.Email.ToUpperInvariant();
					user.PasswordHash = passwordHasher.HashPassword(user, seed.Password);
					user.ConcurrencyStamp = Guid.NewGuid().ToString();
					user.SecurityStamp = Guid.NewGuid().ToString();
					updated = true;
				}
				users.Add(user);
			}

			if (updated) await context.SaveChangesAsync();

			await AssignRolesToUsers(userManager, users);
		}
	}
}

