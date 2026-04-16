using System.Collections.Generic;
using System.Linq;

namespace BG3BuildPlanner.Data.Mock
{
    public class SkillMockRepository
    {
        private readonly List<Skill> _skills;
        private int _nextId;

        public SkillMockRepository()
        {
            _skills = new List<Skill>
            {
                new Skill { Id = 1, Name = "Sneak Attack", Description = "Rogue burst damage from the shadows.", RequiredLevel = 3 },
                new Skill { Id = 2, Name = "Cunning Action", Description = "Dash or disengage to reposition every turn.", RequiredLevel = 4 },
                new Skill { Id = 3, Name = "Hide", Description = "Slip out of sight to set up advantage.", RequiredLevel = 1 },
                new Skill { Id = 4, Name = "Disengage", Description = "Escape melee without provoking attacks.", RequiredLevel = 2 },
                new Skill { Id = 5, Name = "Dash", Description = "Close gaps quickly to keep pressure on casters.", RequiredLevel = 1 },
                new Skill { Id = 6, Name = "Bless", Description = "Buff allies for attack and saves.", RequiredLevel = 2 },
                new Skill { Id = 7, Name = "Spirit Guardians", Description = "Radiant aura that shreds clustered foes.", RequiredLevel = 5 },
                new Skill { Id = 8, Name = "Sanctuary", Description = "Buy time for allies under pressure.", RequiredLevel = 1 },
                new Skill { Id = 9, Name = "Healing Word", Description = "Fast ranged pickup to prevent downs.", RequiredLevel = 1 },
                new Skill { Id = 10, Name = "Guiding Bolt", Description = "Big radiant hit that sets up advantage.", RequiredLevel = 1 },
                new Skill { Id = 11, Name = "Eldritch Blast", Description = "Signature force beam with invocations.", RequiredLevel = 2 },
                new Skill { Id = 12, Name = "Darkness", Description = "Control space and pair with Devil's Sight.", RequiredLevel = 4 },
                new Skill { Id = 13, Name = "Hex", Description = "Reliable damage boost and debuff pressure.", RequiredLevel = 1 },
                new Skill { Id = 14, Name = "Armor of Agathys", Description = "Durable temp HP that punishes attackers.", RequiredLevel = 1 }
            };

            _nextId = _skills.Max(s => s.Id) + 1;
        }

        public IEnumerable<Skill> GetAll()
        {
            return _skills;
        }

        public Skill? GetById(int id)
        {
            return _skills.FirstOrDefault(s => s.Id == id);
        }

        public Skill Add(Skill skill)
        {
            skill.Id = _nextId++;
            _skills.Add(skill);
            return skill;
        }

        public bool Update(Skill updatedSkill)
        {
            var existing = GetById(updatedSkill.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Name = updatedSkill.Name;
            existing.Description = updatedSkill.Description;
            existing.RequiredLevel = updatedSkill.RequiredLevel;
            existing.Builds = updatedSkill.Builds;

            return true;
        }

        public bool Delete(int id)
        {
            var skill = GetById(id);
            if (skill == null)
            {
                return false;
            }

            _skills.Remove(skill);
            return true;
        }
    }
}
