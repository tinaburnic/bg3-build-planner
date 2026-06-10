using System;
using System.ComponentModel.DataAnnotations;

namespace BG3BuildPlanner.Data
{
    public class UserProfileFile
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;

        [Required]
        [MaxLength(260)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(260)]
        public string StoredFileName { get; set; } = string.Empty;

        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }

        [Required]
        [MaxLength(512)]
        public string RelativePath { get; set; } = string.Empty;
    }
}
