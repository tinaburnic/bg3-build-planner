using System;
using System.Collections.Generic;
using System.Linq;

namespace BG3BuildPlanner.Data.Mock
{
    public class CharacterMockRepository
    {
        private readonly List<Character> _characters;
        private int _nextId;

        public CharacterMockRepository()
        {
            _characters = new List<Character>
            {
                new Character
                {
                    Id = 1,
                    Name = "Astarion",
                    PortraitUrl = "/images/characters/astarion.jpg",
                    Race = "High Elf",
                    Background = "Charlatan",
                    Level = 5,
                    CreatedAt = DateTime.UtcNow
                },
                new Character
                {
                    Id = 2,
                    Name = "Shadowheart",
                    PortraitUrl = "/images/characters/shadowheart.jpg",
                    Race = "Half-Elf",
                    Background = "Acolyte",
                    Level = 5,
                    CreatedAt = DateTime.UtcNow
                },
                new Character
                {
                    Id = 3,
                    Name = "Wyll",
                    PortraitUrl = "/images/characters/Wyll.jpg",
                    Race = "Human",
                    Background = "Noble",
                    Level = 5,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _nextId = _characters.Max(c => c.Id) + 1;
        }

        public IEnumerable<Character> GetAll()
        {
            return _characters;
        }

        public Character? GetById(int id)
        {
            return _characters.FirstOrDefault(c => c.Id == id);
        }

        public Character Add(Character character)
        {
            character.Id = _nextId++;
            character.CreatedAt = character.CreatedAt == default ? DateTime.UtcNow : character.CreatedAt;

            _characters.Add(character);
            return character;
        }

        public bool Update(Character updatedCharacter)
        {
            var existing = GetById(updatedCharacter.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Name = updatedCharacter.Name;
            existing.PortraitUrl = updatedCharacter.PortraitUrl;
            existing.Race = updatedCharacter.Race;
            existing.Background = updatedCharacter.Background;
            existing.Level = updatedCharacter.Level;
            existing.Builds = updatedCharacter.Builds;

            return true;
        }

        public bool Delete(int id)
        {
            var character = GetById(id);
            if (character == null)
            {
                return false;
            }

            _characters.Remove(character);
            return true;
        }
    }
}
