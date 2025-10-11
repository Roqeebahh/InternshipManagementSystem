using InternshipManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternshipManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Intern> Interns { get; set; }
        public DbSet<DailyLog> DailyLogs { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<DailyLog>()
                .HasOne(d => d.Intern)
                .WithMany(i => i.DailyLogs)
                .HasForeignKey(d => d.InternId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Intern)
                .WithMany(i => i.Evaluations)
                .HasForeignKey(e => e.InternId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Supervisor)
                .WithMany(a => a.Evaluations)
                .HasForeignKey(e => e.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Intern)
                .WithMany(i => i.Projects)
                .HasForeignKey(p => p.InternId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            modelBuilder.Entity<AdminUser>().HasData
            (
                new AdminUser
                {
                    AdminUserId = 1,
                    FullName = "System Administrator",
                    Email = "admin@internship.com",
                    PasswordHash = "$2a$11$DIietdEeYJZpixm7SAb/cuezWNsBeUTVq6MxRKMY2ZmmyCUIH0mR2",
                    Role = "Admin",
                    CreatedAt = new DateTime(2025, 10, 4),
                    IsActive = true
                }
            );
        }
    }
}

