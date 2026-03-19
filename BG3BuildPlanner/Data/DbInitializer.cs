using System;
using System.Collections.Generic;
using System.Linq;
using BG3BuildPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BG3BuildPlanner.Data
{
	public static class DbInitializer
	{
		public static void Initialize(ApplicationDbContext context)
		{
			context.Database.EnsureCreated();

			if (context.Characters.Any())
			{

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
							CreatedAt = timestamp,
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
							CreatedAt = timestamp,
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
							CreatedAt = timestamp,
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
						}
					}
				};
			

			    context.Characters.AddRange(astarion, shadowheart, Wyll);
			    context.SaveChanges();
		    }
        }
	}
}
