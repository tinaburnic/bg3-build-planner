using System;

namespace BG3BuildPlanner.Models.Shared
{
    public class DateTimePickerModel
    {
        public string InputId { get; set; } = string.Empty;
        public string InputName { get; set; } = string.Empty;
        public string Label { get; set; } = "Date and time";
        public DateTime? Value { get; set; }
        public string Placeholder { get; set; } = "Select date and time";
        public bool Required { get; set; }
        public DateTimePickerFormat Format { get; set; } = DateTimePickerFormat.En;
    }

    public enum DateTimePickerFormat
    {
        En,
        Hr
    }
}
