using System.ComponentModel.DataAnnotations;
using BG3BuildPlanner.Data;

namespace BG3BuildPlanner.Models.Item
{
    public class ItemEditModel
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public ItemType Type { get; set; }

        [Required]
        [StringLength(80)]
        public string Rarity { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Power { get; set; }
    }
}
