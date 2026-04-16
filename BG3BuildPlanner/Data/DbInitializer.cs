using System;
using System.Collections.Generic;
using System.Linq;
using BG3BuildPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

			var timestamp = DateTime.UtcNow;

			    var astarion = new Character
				{
					Name = "Astarion",
					Race = "High Elf",
					Background = "Charlatan",
					Level = 5,
					CreatedAt = timestamp,
					Builds = new List<Build>
					{
						new Build
						{
							Title = "Crimson Stalker",
							Description = "Bleed enemies dry with dual daggers and relentless sneak attacks.",
							Difficulty = Difficulty.Hard,
							CreatedAt = timestamp.AddMinutes(-60),
							Skills = new List<Skill>
							{
								new Skill { Name = "Sneak Attack", Description = "Rogue burst damage from the shadows.", RequiredLevel = 3 },
								new Skill { Name = "Cunning Action", Description = "Dash or disengage to reposition every turn.", RequiredLevel = 4 }
							},
							Items = new List<Item>
							{
								new Item { Name = "Blade of First Blood", Type = ItemType.Weapon, Rarity = "Very Rare", Power = 18 },
								new Item { Name = "Shadeclasp Cloak", Type = ItemType.Accessory, Rarity = "Rare", Power = 12 }
							},
							Ratings = new List<Rating>
							{
								new Rating { Score = 5, Comment = "Deletes priority targets instantly.", CreatedAt = timestamp },
								new Rating { Score = 4, Comment = "Needs careful positioning on Tactician.", CreatedAt = timestamp }
							}
						},
						new Build
						{
							Title = "Silken Infiltrator",
							Description = "Lock down fights with poisons, stealth, and surgical crowd control.",
							Difficulty = Difficulty.Normal,
							CreatedAt = timestamp.AddMinutes(-30),
							Skills = new List<Skill>
							{
								new Skill { Name = "Hide", Description = "Slip out of sight to set up advantage.", RequiredLevel = 1 },
								new Skill { Name = "Disengage", Description = "Escape melee without provoking attacks.", RequiredLevel = 2 }
							},
							Items = new List<Item>
							{
								new Item { Name = "Venomkiss Dagger", Type = ItemType.Weapon, Rarity = "Rare", Power = 15 },
								new Item { Name = "Whisperstep Boots", Type = ItemType.Armor, Rarity = "Uncommon", Power = 9 }
							},
							Ratings = new List<Rating>
							{
								new Rating { Score = 4, Comment = "Very consistent opener and clean escapes.", CreatedAt = timestamp },
								new Rating { Score = 4, Comment = "Great in cramped maps where stealth matters.", CreatedAt = timestamp }
							}
						},
						new Build
						{
							Title = "Nightblade Duellist",
							Description = "Single-target duelist that spikes damage with perfectly-timed strikes.",
							Difficulty = Difficulty.Tactician,
							CreatedAt = timestamp,
							Skills = new List<Skill>
							{
								new Skill { Name = "Dash", Description = "Close gaps quickly to keep pressure on casters.", RequiredLevel = 1 },
								new Skill { Name = "Sneak Attack", Description = "Capitalize on advantage for heavy burst.", RequiredLevel = 3 }
							},
							Items = new List<Item>
							{
								new Item { Name = "Duelist's Fang", Type = ItemType.Weapon, Rarity = "Very Rare", Power = 19 },
								new Item { Name = "Ring of Sudden Silence", Type = ItemType.Accessory, Rarity = "Rare", Power = 11 }
							},
							Ratings = new List<Rating>
							{
								new Rating { Score = 5, Comment = "Punishes isolated targets hard.", CreatedAt = timestamp },
								new Rating { Score = 3, Comment = "Positioning mistakes get punished on Tactician.", CreatedAt = timestamp }
							}
						}
					}
				};

				var shadowheart = new Character
				{
					Name = "Shadowheart",
					Race = "Half-Elf",
					Background = "Acolyte",
					Level = 5,
					CreatedAt = timestamp,
					Builds = new List<Build>
					{
						new Build
						{
							Title = "Twilight Guardian",
							Description = "Protect allies with radiant shields and crowd control.",
							Difficulty = Difficulty.Normal,
							CreatedAt = timestamp.AddMinutes(-55),
							Skills = new List<Skill>
							{
								new Skill { Name = "Bless", Description = "Buff allies for attack and saves.", RequiredLevel = 2 },
								new Skill { Name = "Spirit Guardians", Description = "Radiant aura that shreds clustered foes.", RequiredLevel = 5 }
							},
							Items = new List<Item>
							{
								new Item { Name = "Luminous Mace", Type = ItemType.Weapon, Rarity = "Rare", Power = 16 },
								new Item { Name = "Justiciar's Armor", Type = ItemType.Armor, Rarity = "Very Rare", Power = 20 }
							},
							Ratings = new List<Rating>
							{
								new Rating { Score = 5, Comment = "Unkillable frontline support.", CreatedAt = timestamp },
								new Rating { Score = 3, Comment = "Damage ramps slowly before level 5.", CreatedAt = timestamp }
							}
						},
						new Build
						{
							Title = "Sanctuary Weaver",
							Description = "Pure support cleric focusing on saves, healing, and emergency resets.",
							Difficulty = Difficulty.Hard,
							CreatedAt = timestamp.AddMinutes(-25),
							Skills = new List<Skill>
							{
								new Skill { Name = "Sanctuary", Description = "Buy time for allies under pressure.", RequiredLevel = 1 },
								new Skill { Name = "Healing Word", Description = "Fast ranged pickup to prevent downs.", RequiredLevel = 1 }
							},
							Items = new List<Item>
							{
								new Item { Name = "Chalice of Gentle Light", Type = ItemType.Accessory, Rarity = "Rare", Power = 14 },
								new Item { Name = "Blessed Vestments", Type = ItemType.Armor, Rarity = "Uncommon", Power = 11 }
							},
							Ratings = new List<Rating>
							{
								new Rating { Score = 4, Comment = "Carries tough fights with clutch saves.", CreatedAt = timestamp },
								new Rating { Score = 4, Comment = "Not flashy, but extremely safe.", CreatedAt = timestamp }
							}
						},
						new Build
						{
							Title = "Radiant Purifier",
							Description = "Aggressive radiant caster that melts undead and clustered enemies.",
							Difficulty = Difficulty.Tactician,
							CreatedAt = timestamp,
							Skills = new List<Skill>
							{
								new Skill { Name = "Guiding Bolt", Description = "Big radiant hit that sets up advantage.", RequiredLevel = 1 },
								new Skill { Name = "Spirit Guardians", Description = "Constant radiant pressure in melee range.", RequiredLevel = 5 }
							},
							Items = new List<Item>
							{
								new Item { Name = "Sunflare Symbol", Type = ItemType.Accessory, Rarity = "Very Rare", Power = 18 },
								new Item { Name = "Dawnward Shield", Type = ItemType.Armor, Rarity = "Rare", Power = 15 }
							},
							Ratings = new List<Rating>
							{
								new Rating { Score = 4, Comment = "High impact once it gets rolling.", CreatedAt = timestamp },
								new Rating { Score = 4, Comment = "Demands positioning but rewards it.", CreatedAt = timestamp }
							}
						}
					}
				};
                
                var Wyll = new Character
				{
					Name = "Wyll",
					Race = "Human",
					Background = "Noble",
					Level = 5,
					CreatedAt = timestamp,
					Builds = new List<Build>
					{
						new Build
						{
							Title = "Blade of the Frontier",
							Description = "Hybrid warlock gish weaving Eldritch Blast with pact blade strikes.",
							Difficulty = Difficulty.Tactician,
							CreatedAt = timestamp.AddMinutes(-50),
							Skills = new List<Skill>
							{
								new Skill { Name = "Eldritch Blast", Description = "Signature force beam with invocations.", RequiredLevel = 2 },
								new Skill { Name = "Darkness", Description = "Control space and pair with Devil's Sight.", RequiredLevel = 4 }
							},
							Items = new List<Item>
							{
								new Item { Name = "Infernal Rapier", Type = ItemType.Weapon, Rarity = "Rare", Power = 17 },
								new Item { Name = "Pactbound Sigil", Type = ItemType.Accessory, Rarity = "Uncommon", Power = 10 }
							},
							Ratings = new List<Rating>
							{
								new Rating { Score = 4, Comment = "Excellent single-target control.", CreatedAt = timestamp },
								new Rating { Score = 4, Comment = "Resource hungry without short rests.", CreatedAt = timestamp }
							}
						},
						new Build
						{
							Title = "Hellfire Artillerist",
							Description = "Ranged blaster warlock that deletes threats with force and fire.",
							Difficulty = Difficulty.Hard,
							CreatedAt = timestamp.AddMinutes(-20),
							Skills = new List<Skill>
							{
								new Skill { Name = "Hex", Description = "Reliable damage boost and debuff pressure.", RequiredLevel = 1 },
								new Skill { Name = "Eldritch Blast", Description = "Scales well and fits every turn.", RequiredLevel = 2 }
							},
							Items = new List<Item>
							{
								new Item { Name = "Gleamshot Focus", Type = ItemType.Accessory, Rarity = "Rare", Power = 15 },
								new Item { Name = "Ashen Mantle", Type = ItemType.Armor, Rarity = "Uncommon", Power = 12 }
							},
							Ratings = new List<Rating>
							{
								new Rating { Score = 5, Comment = "Simple, strong, and always online.", CreatedAt = timestamp },
								new Rating { Score = 3, Comment = "Wants positioning and sight lines.", CreatedAt = timestamp }
							}
						},
						new Build
						{
							Title = "Pactblade Duelist",
							Description = "Melee pact build that trades safely and finishes with bursts.",
							Difficulty = Difficulty.Normal,
							CreatedAt = timestamp,
							Skills = new List<Skill>
							{
								new Skill { Name = "Armor of Agathys", Description = "Durable temp HP that punishes attackers.", RequiredLevel = 1 },
								new Skill { Name = "Darkness", Description = "Create favorable melee trades.", RequiredLevel = 4 }
							},
							Items = new List<Item>
							{
								new Item { Name = "Frontier's Pactblade", Type = ItemType.Weapon, Rarity = "Very Rare", Power = 18 },
								new Item { Name = "Signet of the Hells", Type = ItemType.Accessory, Rarity = "Rare", Power = 13 }
							},
							Ratings = new List<Rating>
							{
								new Rating { Score = 4, Comment = "Feels great in mixed melee/ranged parties.", CreatedAt = timestamp },
								new Rating { Score = 4, Comment = "Very flexible turn-to-turn.", CreatedAt = timestamp }
							}
						}
					}
				};
			
			context.Characters.AddRange(astarion, shadowheart, Wyll);
			context.SaveChanges();
        }
	}
}

