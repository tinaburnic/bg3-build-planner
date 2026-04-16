using System.Collections.Generic;
using System.Linq;

namespace BG3BuildPlanner.Data.Mock
{
    public class SkillMockRepository
    {
        private readonly List<Skill> _skills;
        private int _nextId;

        private string GetImageUrlForSkill(string skillName)
        {
            var imageName = skillName.Replace(" ", "_") + ".jpg";
            // Handle known mismatches
            if (skillName == "Armor of Agathys")
            {
                imageName = "Armour_of_Agathys.jpg";
            }
            else if (skillName == "Sneak Attack")
            {
                imageName = "Sneak_Attack_Ranged.jpg";
            }
            else if (skillName == "Cunning Action")
            {
                imageName = "Dash_bonus_action.jpg"; // Representative image
            }

            // Fallback can be a generic image if one exists, or empty
            return $"/images/skills/{imageName}";
        }

        public SkillMockRepository()
        {
            var skillsData = new List<(string Name, string Description, int RequiredLevel)>
            {
                ("Sneak Attack", "Rogue burst damage from the shadows.", 3),
                ("Cunning Action", "Dash or disengage to reposition every turn.", 4),
                ("Hide", "Slip out of sight to set up advantage.", 1),
                ("Disengage", "Escape melee without provoking attacks.", 2),
                ("Dash", "Close gaps quickly to keep pressure on casters.", 1),
                ("Bless", "Buff allies for attack and saves.", 2),
                ("Spirit Guardians", "Radiant aura that shreds clustered foes.", 5),
                ("Sanctuary", "Buy time for allies under pressure.", 1),
                ("Healing Word", "Fast ranged pickup to prevent downs.", 1),
                ("Guiding Bolt", "Big radiant hit that sets up advantage.", 1),
                ("Eldritch Blast", "Signature force beam with invocations.", 2),
                ("Darkness", "Control space and pair with Devil's Sight.", 4),
                ("Hex", "Reliable damage boost and debuff pressure.", 1),
                ("Armor of Agathys", "Durable temp HP that punishes attackers.", 1)
            };

            _skills = skillsData.Select((s, index) => new Skill
            {
                Id = index + 1,
                Name = s.Name,
                Description = s.Description,
                RequiredLevel = s.RequiredLevel,
                ImageUrl = GetImageUrlForSkill(s.Name)
            }).ToList();

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
            if (string.IsNullOrWhiteSpace(skill.ImageUrl))
            {
                skill.ImageUrl = GetImageUrlForSkill(skill.Name);
            }
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
            existing.ImageUrl = string.IsNullOrWhiteSpace(updatedSkill.ImageUrl)
                ? GetImageUrlForSkill(updatedSkill.Name)
                : updatedSkill.ImageUrl;
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
