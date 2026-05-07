using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Data
{   
    public class Item
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public ItemType Type { get; set; }
    public required string Rarity { get; set; }
    public int Power { get; set; }

    public virtual ICollection<Build> Builds { get; set; } = new List<Build>();
}
}

