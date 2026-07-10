using System.Collections.Generic;

namespace BG3BuildPlanner.Models.Search
{
    public class GlobalSearchViewModel
    {
        public string Term { get; init; } = string.Empty;
        public IReadOnlyList<GlobalSearchResultItemViewModel> Characters { get; init; } = new List<GlobalSearchResultItemViewModel>();
        public IReadOnlyList<GlobalSearchResultItemViewModel> Builds { get; init; } = new List<GlobalSearchResultItemViewModel>();
        public IReadOnlyList<GlobalSearchResultItemViewModel> Skills { get; init; } = new List<GlobalSearchResultItemViewModel>();
        public IReadOnlyList<GlobalSearchResultItemViewModel> Items { get; init; } = new List<GlobalSearchResultItemViewModel>();

        public bool HasQuery => !string.IsNullOrWhiteSpace(Term);

        public int TotalResults => Characters.Count + Builds.Count + Skills.Count + Items.Count;
    }
}