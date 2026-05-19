using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BG3BuildPlanner.Data
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public int Score { get; set; } // 1 to 5
        public required string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Build")]
        public int BuildId { get; set; }
        public required virtual Build Build { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public required virtual User User { get; set; }
    }
}

