using System.ComponentModel.DataAnnotations;

namespace InternshipManagementSystem.Models
{
    public class AdminUser
    {
        public int AdminUserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } // Admin, Supervisor

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}

