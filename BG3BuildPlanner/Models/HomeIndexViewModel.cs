using System.Collections.Generic;
using BG3BuildPlanner.Data;

namespace BG3BuildPlanner.Models
{
    public class HomeIndexViewModel
    {
        public IReadOnlyList<Build> FeaturedBuilds { get; init; } = new List<Build>();
    }
}
