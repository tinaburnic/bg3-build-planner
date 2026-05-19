using System.Collections.Generic;
using BuildEntity = BG3BuildPlanner.Data.Build;

namespace BG3BuildPlanner.Models
{
    public class HomeIndexViewModel
    {
        public IReadOnlyList<BuildEntity> FeaturedBuilds { get; init; } = new List<BuildEntity>();
    }
}
