using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InternshipManagementSystem.Data;
using InternshipManagementSystem.Models.ViewModels;
using System.Security.Claims;

namespace InternshipManagementSystem.Controllers
{
    [Authorize(Roles = "Intern")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var internId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var viewModel = new DashboardViewModel
            {
                TotalLogs = await _context.DailyLogs.CountAsync(d => d.InternId == internId),
                TotalEvaluations = await _context.Evaluations.CountAsync(e => e.InternId == internId),
                TotalProjects = await _context.Projects.CountAsync(p => p.InternId == internId),
                AverageScore = await _context.Evaluations
                    .Where(e => e.InternId == internId)
                    .AverageAsync(e => (double?)(e.Punctuality + e.Teamwork + e.SkillLevel) / 3) ?? 0,
                RecentLogs = await _context.DailyLogs
                    .Where(d => d.InternId == internId)
                    .OrderByDescending(d => d.LogDate)
                    .Take(5)
                    .ToListAsync(),
                RecentEvaluations = await _context.Evaluations
                    .Include(e => e.Supervisor)
                    .Where(e => e.InternId == internId)
                    .OrderByDescending(e => e.EvaluationDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}