using BG3BuildPlanner.Data;

namespace BG3BuildPlanner.Models.Dto
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ItemType Type { get; set; }
        public string Rarity { get; set; } = string.Empty;
        public int Power { get; set; }
    }
}