using System;
using System.Collections.Generic;
using System.Linq;

namespace BG3BuildPlanner.Data.Mock
{
    public class BuildMockRepository
    {
        private readonly List<Build> _builds;
        private int _nextId;
        private readonly Dictionary<int, Character> _charactersById;
        private readonly User _defaultUser;
		private sealed record RatingSeed(int Score, string Comment, DateTime CreatedAt);

        public BuildMockRepository()
        {
            var timestamp = DateTime.UtcNow;
            var characterRepository = new CharacterMockRepository();

            _charactersById = characterRepository.GetAll()
                .ToDictionary(c => c.Id, c => new Character
                {
                    Id = c.Id,
                    Name = c.Name,
                    PortraitUrl = c.PortraitUrl,
                    Race = c.Race,
                    Background = c.Background,
                    Level = c.Level,
                    CreatedAt = c.CreatedAt
                });

            _defaultUser = new User
            {
                Username = "demo",
                Email = "demo@example.com",
                PasswordHash = "demo"
            };

            _builds = new List<Build>
            {
                CreateBuild(
                    id: 1,
                    characterId: 1,
                    title: "Crimson Stalker",
                    description: "Bleed enemies dry with dual daggers and relentless sneak attacks.",
                    difficulty: Difficulty.Balanced,
                    createdAt: timestamp.AddMinutes(-60),
                    skills: new List<Skill>
                    {
                        new Skill { Name = "Sneak Attack", Description = "Rogue burst damage from the shadows.", RequiredLevel = 3, ImageUrl = string.Empty },
                        new Skill { Name = "Cunning Action", Description = "Dash or disengage to reposition every turn.", RequiredLevel = 4, ImageUrl = string.Empty }
                    },
                    items: new List<Item>
                    {
                        new Item { Name = "Blade of First Blood", Type = ItemType.Weapon, Rarity = "Very Rare", Power = 18 },
                        new Item { Name = "Shadeclasp Cloak", Type = ItemType.Accessory, Rarity = "Rare", Power = 12 }
                    },
                    ratings: new List<RatingSeed>
                    {
                        new RatingSeed(5, "Deletes priority targets instantly.", timestamp),
                        new RatingSeed(4, "Needs careful positioning on Tactician.", timestamp)
                    }),
                CreateBuild(
                    id: 2,
                    characterId: 1,
                    title: "Silken Infiltrator",
                    description: "Lock down fights with poisons, stealth, and surgical crowd control.",
                    difficulty: Difficulty.Explorer,
                    createdAt: timestamp.AddMinutes(-30),
                    skills: new List<Skill>
                    {
                        new Skill { Name = "Hide", Description = "Slip out of sight to set up advantage.", RequiredLevel = 1, ImageUrl = string.Empty },
                        new Skill { Name = "Disengage", Description = "Escape melee without provoking attacks.", RequiredLevel = 2, ImageUrl = string.Empty }
                    },
                    items: new List<Item>
                    {
                        new Item { Name = "Venomkiss Dagger", Type = ItemType.Weapon, Rarity = "Rare", Power = 15 },
                        new Item { Name = "Whisperstep Boots", Type = ItemType.Armor, Rarity = "Uncommon", Power = 9 }
                    },
                    ratings: new List<RatingSeed>
                    {
                        new RatingSeed(4, "Very consistent opener and clean escapes.", timestamp),
                        new RatingSeed(4, "Great in cramped maps where stealth matters.", timestamp)
                    }),
                CreateBuild(
                    id: 3,
                    characterId: 1,
                    title: "Nightblade Duellist",
                    description: "Single-target duelist that spikes damage with perfectly-timed strikes.",
                    difficulty: Difficulty.Tactician,
                    createdAt: timestamp,
                    skills: new List<Skill>
                    {
                        new Skill { Name = "Dash", Description = "Close gaps quickly to keep pressure on casters.", RequiredLevel = 1, ImageUrl = string.Empty },
                        new Skill { Name = "Sneak Attack", Description = "Capitalize on advantage for heavy burst.", RequiredLevel = 3, ImageUrl = string.Empty }
                    },
                    items: new List<Item>
                    {
                        new Item { Name = "Duelist's Fang", Type = ItemType.Weapon, Rarity = "Very Rare", Power = 19 },
                        new Item { Name = "Ring of Sudden Silence", Type = ItemType.Accessory, Rarity = "Rare", Power = 11 }
                    },
                    ratings: new List<RatingSeed>
                    {
                        new RatingSeed(5, "Punishes isolated targets hard.", timestamp),
                        new RatingSeed(3, "Positioning mistakes get punished on Tactician.", timestamp)
                    }),
                CreateBuild(
                    id: 4,
                    characterId: 2,
                    title: "Twilight Guardian",
                    description: "Protect allies with radiant shields and crowd control.",
                    difficulty: Difficulty.Explorer,
                    createdAt: timestamp.AddMinutes(-55),
                    skills: new List<Skill>
                    {
                        new Skill { Name = "Bless", Description = "Buff allies for attack and saves.", RequiredLevel = 2, ImageUrl = string.Empty },
                        new Skill { Name = "Spirit Guardians", Description = "Radiant aura that shreds clustered foes.", RequiredLevel = 5, ImageUrl = string.Empty }
                    },
                    items: new List<Item>
                    {
                        new Item { Name = "Luminous Mace", Type = ItemType.Weapon, Rarity = "Rare", Power = 16 },
                        new Item { Name = "Justiciar's Armor", Type = ItemType.Armor, Rarity = "Very Rare", Power = 20 }
                    },
                    ratings: new List<RatingSeed>
                    {
                        new RatingSeed(5, "Unkillable frontline support.", timestamp),
                        new RatingSeed(3, "Damage ramps slowly before level 5.", timestamp)
                    }),
                CreateBuild(
                    id: 5,
                    characterId: 2,
                    title: "Sanctuary Weaver",
                    description: "Pure support cleric focusing on saves, healing, and emergency resets.",
                    difficulty: Difficulty.Balanced,
                    createdAt: timestamp.AddMinutes(-25),
                    skills: new List<Skill>
                    {
                        new Skill { Name = "Sanctuary", Description = "Buy time for allies under pressure.", RequiredLevel = 1, ImageUrl = string.Empty },
                        new Skill { Name = "Healing Word", Description = "Fast ranged pickup to prevent downs.", RequiredLevel = 1, ImageUrl = string.Empty }
                    },
                    items: new List<Item>
                    {
                        new Item { Name = "Chalice of Gentle Light", Type = ItemType.Accessory, Rarity = "Rare", Power = 14 },
                        new Item { Name = "Blessed Vestments", Type = ItemType.Armor, Rarity = "Uncommon", Power = 11 }
                    },
                    ratings: new List<RatingSeed>
                    {
                        new RatingSeed(4, "Carries tough fights with clutch saves.", timestamp),
                        new RatingSeed(4, "Not flashy, but extremely safe.", timestamp)
                    }),
                CreateBuild(
                    id: 6,
                    characterId: 2,
                    title: "Radiant Purifier",
                    description: "Aggressive radiant caster that melts undead and clustered enemies.",
                    difficulty: Difficulty.Tactician,
                    createdAt: timestamp,
                    skills: new List<Skill>
                    {
                        new Skill { Name = "Guiding Bolt", Description = "Big radiant hit that sets up advantage.", RequiredLevel = 1, ImageUrl = string.Empty },
                        new Skill { Name = "Spirit Guardians", Description = "Constant radiant pressure in melee range.", RequiredLevel = 5, ImageUrl = string.Empty }
                    },
                    items: new List<Item>
                    {
                        new Item { Name = "Sunflare Symbol", Type = ItemType.Accessory, Rarity = "Very Rare", Power = 18 },
                        new Item { Name = "Dawnward Shield", Type = ItemType.Armor, Rarity = "Rare", Power = 15 }
                    },
                    ratings: new List<RatingSeed>
                    {
                        new RatingSeed(4, "High impact once it gets rolling.", timestamp),
                        new RatingSeed(4, "Demands positioning but rewards it.", timestamp)
                    }),
                CreateBuild(
                    id: 7,
                    characterId: 3,
                    title: "Blade of the Frontier",
                    description: "Hybrid warlock gish weaving Eldritch Blast with pact blade strikes.",
                    difficulty: Difficulty.Tactician,
                    createdAt: timestamp.AddMinutes(-50),
                    skills: new List<Skill>
                    {
                        new Skill { Name = "Eldritch Blast", Description = "Signature force beam with invocations.", RequiredLevel = 2, ImageUrl = string.Empty },
                        new Skill { Name = "Darkness", Description = "Control space and pair with Devil's Sight.", RequiredLevel = 4, ImageUrl = string.Empty }
                    },
                    items: new List<Item>
                    {
                        new Item { Name = "Infernal Rapier", Type = ItemType.Weapon, Rarity = "Rare", Power = 17 },
                        new Item { Name = "Pactbound Sigil", Type = ItemType.Accessory, Rarity = "Uncommon", Power = 10 }
                    },
                    ratings: new List<RatingSeed>
                    {
                        new RatingSeed(4, "Excellent single-target control.", timestamp),
                        new RatingSeed(4, "Resource hungry without short rests.", timestamp)
                    }),
                CreateBuild(
                    id: 8,
                    characterId: 3,
                    title: "Hellfire Artillerist",
                    description: "Ranged blaster warlock that deletes threats with force and fire.",
                    difficulty: Difficulty.Balanced,
                    createdAt: timestamp.AddMinutes(-20),
                    skills: new List<Skill>
                    {
                        new Skill { Name = "Hex", Description = "Reliable damage boost and debuff pressure.", RequiredLevel = 1, ImageUrl = string.Empty },
                        new Skill { Name = "Eldritch Blast", Description = "Scales well and fits every turn.", RequiredLevel = 2, ImageUrl = string.Empty }
                    },
                    items: new List<Item>
                    {
                        new Item { Name = "Gleamshot Focus", Type = ItemType.Accessory, Rarity = "Rare", Power = 15 },
                        new Item { Name = "Ashen Mantle", Type = ItemType.Armor, Rarity = "Uncommon", Power = 12 }
                    },
                    ratings: new List<RatingSeed>
                    {
                        new RatingSeed(5, "Simple, strong, and always online.", timestamp),
                        new RatingSeed(3, "Wants positioning and sight lines.", timestamp)
                    }),
                CreateBuild(
                    id: 9,
                    characterId: 3,
                    title: "Pactblade Duelist",
                    description: "Melee pact build that trades safely and finishes with bursts.",
                    difficulty: Difficulty.Explorer,
                    createdAt: timestamp,
                    skills: new List<Skill>
                    {
                        new Skill { Name = "Armor of Agathys", Description = "Durable temp HP that punishes attackers.", RequiredLevel = 1, ImageUrl = string.Empty },
                        new Skill { Name = "Darkness", Description = "Create favorable melee trades.", RequiredLevel = 4, ImageUrl = string.Empty }
                    },
                    items: new List<Item>
                    {
                        new Item { Name = "Frontier's Pactblade", Type = ItemType.Weapon, Rarity = "Very Rare", Power = 18 },
                        new Item { Name = "Signet of the Hells", Type = ItemType.Accessory, Rarity = "Rare", Power = 13 }
                    },
                    ratings: new List<RatingSeed>
                    {
                        new RatingSeed(4, "Feels great in mixed melee/ranged parties.", timestamp),
                        new RatingSeed(4, "Very flexible turn-to-turn.", timestamp)
                    })
            };

            _nextId = _builds.Max(b => b.Id) + 1;
        }

        public IEnumerable<Build> GetAll()
        {
            return _builds;
        }

        public IEnumerable<Build> GetByCharacterId(int characterId)
        {
            return _builds.Where(b => b.CharacterId == characterId);
        }

        public Build? GetById(int id)
        {
            return _builds.FirstOrDefault(b => b.Id == id);
        }

        public Build Add(Build build)
        {
            build.Id = _nextId++;
            build.CreatedAt = build.CreatedAt == default ? DateTime.UtcNow : build.CreatedAt;

            _builds.Add(build);
            return build;
        }

        public bool Update(Build updatedBuild)
        {
            var existing = GetById(updatedBuild.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Title = updatedBuild.Title;
            existing.Description = updatedBuild.Description;
            existing.Difficulty = updatedBuild.Difficulty;
            existing.CharacterId = updatedBuild.CharacterId;
            existing.Character = updatedBuild.Character;
            existing.Skills = updatedBuild.Skills;
            existing.Items = updatedBuild.Items;
            existing.Ratings = updatedBuild.Ratings;

            return true;
        }

        public bool Delete(int id)
        {
            var build = GetById(id);
            if (build == null)
            {
                return false;
            }

            _builds.Remove(build);
            return true;
        }

        private Build CreateBuild(
            int id,
            int characterId,
            string title,
            string description,
            Difficulty difficulty,
            DateTime createdAt,
            List<Skill> skills,
            List<Item> items,
            List<RatingSeed> ratings)
        {
            var build = new Build
            {
                Id = id,
                CharacterId = characterId,
                Title = title,
                Description = description,
                Difficulty = difficulty,
                CreatedAt = createdAt,
                User = _defaultUser,
                Character = _charactersById.TryGetValue(characterId, out var character)
                    ? character
                    : new Character
                    {
                        Id = characterId,
                        Name = $"Character {characterId}",
                        Race = "Unknown",
                        Background = "Unknown",
                        Level = 1,
                        CreatedAt = DateTime.UtcNow
                    },
                Skills = skills,
                Items = items,
                Ratings = new List<Rating>()
            };

			foreach (var rating in ratings)
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
        }
    }
}
