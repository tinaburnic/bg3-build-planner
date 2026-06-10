using System;

namespace BG3BuildPlanner.Models.Dto
{
    public class ProfileFileDto
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string RelativePath { get; set; } = string.Empty;
        public bool IsCurrentProfileImage { get; set; }
    }
}
