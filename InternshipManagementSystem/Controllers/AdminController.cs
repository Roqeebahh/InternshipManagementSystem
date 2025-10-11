using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InternshipManagementSystem.Data;
using InternshipManagementSystem.Models;
using InternshipManagementSystem.Models.ViewModels;
using InternshipManagementSystem.Services;

namespace InternshipManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReportService _reportService;

        public AdminController(ApplicationDbContext context, IReportService reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalInterns = await _context.Interns.CountAsync(),
                ActiveInterns = await _context.Interns.CountAsync(i => i.IsActive),
                TotalLogs = await _context.DailyLogs.CountAsync(),
                TotalEvaluations = await _context.Evaluations.CountAsync(),
                RecentInterns = await _context.Interns
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                RecentLogs = await _context.DailyLogs
                    .Include(d => d.Intern)
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(10)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Interns(string? search, string sort = "name")
        {
            var query = _context.Interns.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i => i.FullName.Contains(search) ||
                                        i.Email.Contains(search) ||
                                        i.Institution.Contains(search));
            }

            switch (sort.ToLower())
            {
                case "email":
                    query = query.OrderBy(i => i.Email);
                    break;
                case "institution":
                    query = query.OrderBy(i => i.Institution);
                    break;
                case "startdate":
                    query = query.OrderBy(i => i.StartDate);
                    break;
                default:
                    query = query.OrderBy(i => i.FullName);
                    break;
            }

            var interns = await query.ToListAsync();
            ViewBag.Search = search;
            ViewBag.Sort = sort;

            return View(interns);
        }

        public async Task<IActionResult> InternDetails(int id)
        {
            var intern = await _context.Interns
                .Include(i => i.DailyLogs)
                .Include(i => i.Evaluations)
                    .ThenInclude(e => e.Supervisor)
                .Include(i => i.Projects)
                .FirstOrDefaultAsync(i => i.InternId == id);

            if (intern == null)
                return NotFound();

            return View(intern);
        }

        public async Task<IActionResult> AllLogs(string? search, DateTime? date)
        {
            var query = _context.DailyLogs.Include(d => d.Intern).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Intern!.FullName.Contains(search) ||
                                        d.Activity.Contains(search));
            }

            if (date.HasValue)
            {
                query = query.Where(d => d.LogDate.Date == date.Value.Date);
            }

            var logs = await query.OrderByDescending(d => d.LogDate).ToListAsync();
            ViewBag.Search = search;
            ViewBag.Date = date?.ToString("yyyy-MM-dd");

            return View(logs);
        }

        public async Task<IActionResult> AllEvaluations()
        {
            var evaluations = await _context.Evaluations
                .Include(e => e.Intern)
                .Include(e => e.Supervisor)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();

            return View(evaluations);
        }

        public async Task<IActionResult> AllProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.Intern)
                .OrderByDescending(p => p.SubmissionDate)
                .ToListAsync();

            return View(projects);
        }

        public async Task<IActionResult> GenerateReport(int internId)
        {
            var intern = await _context.Interns
                .Include(i => i.DailyLogs)
                .Include(i => i.Evaluations)
                    .ThenInclude(e => e.Supervisor)
                .Include(i => i.Projects)
                .FirstOrDefaultAsync(i => i.InternId == internId);

            if (intern == null)
                return NotFound();

            var pdfBytes = await _reportService.GenerateInternshipReport(intern);
            return File(pdfBytes, "application/pdf", $"InternshipReport_{intern.FullName}.pdf");
        }

        public async Task<IActionResult> ExportLogs(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.DailyLogs.Include(d => d.Intern).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(d => d.LogDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(d => d.LogDate <= endDate.Value);

            var logs = await query.OrderBy(d => d.LogDate).ToListAsync();
            var pdfBytes = await _reportService.GenerateLogsReport(logs, startDate, endDate);

            return File(pdfBytes, "application/pdf", "DailyLogsReport.pdf");
        }
    }
}