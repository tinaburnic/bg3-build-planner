namespace BG3BuildPlanner.Models.Search
{
    public class GlobalSearchResultItemViewModel
    {
        public int Id { get; init; }
        public required string Title { get; init; }
        public string? Subtitle { get; init; }
        public string? Description { get; init; }
        public required string Controller { get; init; }
        public required string TypeLabel { get; init; }
    }
}