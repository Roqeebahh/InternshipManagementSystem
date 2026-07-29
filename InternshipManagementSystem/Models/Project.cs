using System.ComponentModel.DataAnnotations;

namespace InternshipManagementSystem.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        [Required]
        public int InternId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? FilePath { get; set; }

        [StringLength(100)]
        public string? FileName { get; set; }

        public virtual Intern? Intern { get; set; }
    }
}
