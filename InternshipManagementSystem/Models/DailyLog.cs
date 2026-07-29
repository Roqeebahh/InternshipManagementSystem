using System;
using System.ComponentModel.DataAnnotations;

namespace InternshipManagementSystem.Models
{
    public class DailyLog
    {
        public int DailyLogId { get; set; }

        [Required]
        public int InternId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime LogDate { get; set; }

        [Required]
        [StringLength(1000)]
        public string Activity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Intern? Intern { get; set; }

    }
}
