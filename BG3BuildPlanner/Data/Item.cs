using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Data
{   
    public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public string Rarity { get; set; }
    public int Power { get; set; }

    public virtual ICollection<Build> Builds { get; set; } = new List<Build>();
}
}

