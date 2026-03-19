using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BG3BuildPlanner.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public int Score { get; set; } // 1 to 5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Build")]
        public int BuildId { get; set; }
        public virtual Build Build { get; set; }
    }
}