namespace BG3BuildPlanner.Models.Shared
{
    public class AutocompleteInputModel
    {
        public string InputId { get; set; } = string.Empty;
        public string InputName { get; set; } = string.Empty;
        public string HiddenInputId { get; set; } = string.Empty;
        public string HiddenInputName { get; set; } = string.Empty;
        public string? InitialText { get; set; }
        public int? InitialId { get; set; }
        public string Label { get; set; } = "Search";
        public string Placeholder { get; set; } = "Type to search";
        public string EndpointUrl { get; set; } = string.Empty;
        public int MinLength { get; set; } = 2;
        public bool Required { get; set; }
        public string RequiredMessage { get; set; } = "Please select a value.";
    }
}
