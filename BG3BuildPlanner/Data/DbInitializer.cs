using System;
using System.Collections.Generic;
using System.Linq;
using BG3BuildPlanner.Data.Mock;

namespace BG3BuildPlanner.Data
{
	public static class DbInitializer
	{
		public static void Initialize(ApplicationDbContext context)
		{
			context.Database.EnsureCreated();

			// DB already seeded.
			if (context.Characters.Any())
			{
				return;
			}

			var user = new User
			{
				Username = "demo",
				Email = "demo@example.com",
				PasswordHash = "demo"
			};

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
				.Select(b =>
				{
					var build = new Build
					{
						Title = b.Title,
						Description = b.Description,
						Difficulty = b.Difficulty,
						CreatedAt = b.CreatedAt,
						Character = charactersById[b.CharacterId],
						User = user,
						Skills = b.Skills.Select(s => GetOrCreateSkill(skillsByName, s)).ToList(),
						Items = b.Items.Select(i => GetOrCreateItem(itemsByKey, i)).ToList(),
						Ratings = new List<Rating>()
					};

					foreach (var rating in b.Ratings)
					{
						build.Ratings.Add(new Rating
						{
							Score = rating.Score,
							Comment = rating.Comment,
							CreatedAt = rating.CreatedAt,
							Build = build
						});
					}

					return build;
				})
				.ToList();

			context.Users.Add(user);
			context.Characters.AddRange(characters);
			context.Skills.AddRange(skillsByName.Values);
			context.Items.AddRange(itemsByKey.Values);
			context.Builds.AddRange(builds);
			context.SaveChanges();
        }

		private static Skill GetOrCreateSkill(
			IDictionary<string, Skill> skillsByName,
			Skill source)
		{
			if (skillsByName.TryGetValue(source.Name, out var existing))
			{
				return existing;
			}

			var created = new Skill
			{
				Name = source.Name,
				Description = source.Description,
				RequiredLevel = source.RequiredLevel,
				ImageUrl = source.ImageUrl
			};
			skillsByName[source.Name] = created;
			return created;
		}

		private static Item GetOrCreateItem(
			IDictionary<string, Item> itemsByKey,
			Item source)
		{
			var key = $"{source.Name}|{source.Type}|{source.Rarity}|{source.Power}";
			if (itemsByKey.TryGetValue(key, out var existing))
			{
				return existing;
			}

			var created = new Item
			{
				Name = source.Name,
				Type = source.Type,
				Rarity = source.Rarity,
				Power = source.Power
			};
			itemsByKey[key] = created;
			return created;
		}
	}
}

