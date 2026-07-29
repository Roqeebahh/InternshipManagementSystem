using System;
using System.ComponentModel.DataAnnotations;

namespace InternshipManagementSystem.Models
{
    public class Evaluation
    {
        public int EvaluationId { get; set; }

        [Required]
        public int InternId { get; set; }

        [Required]
        public int SupervisorId { get; set; }

        [Range(1, 10)]
        public int Punctuality { get; set; }

        [Range(1, 10)]
        public int Teamwork { get; set; }

        [Range(1, 10)]
        public int SkillLevel { get; set; }

        public int Score { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; } = string.Empty;

        public DateTime EvaluationDate { get; set; } = DateTime.Now;

        public virtual Intern? Intern { get; set; }
        public virtual AdminUser? Supervisor { get; set; }
    }
}
