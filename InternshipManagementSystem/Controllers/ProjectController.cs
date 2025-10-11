using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InternshipManagementSystem.Data;
using InternshipManagementSystem.Models;
using System.Security.Claims;

namespace InternshipManagementSystem.Controllers
{
    [Authorize(Roles = "Intern")]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProjectController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var internId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var projects = await _context.Projects
                .Where(p => p.InternId == internId)
                .OrderByDescending(p => p.SubmissionDate)
                .ToListAsync();

            return View(projects);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                model.InternId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                model.SubmissionDate = DateTime.Now;

                // Handle file upload
                if (file != null && file.Length > 0)
                {
                    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "projects");
                    Directory.CreateDirectory(uploadPath);

                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    model.FilePath = $"/uploads/projects/{fileName}";
                    model.FileName = file.FileName;
                }

                _context.Projects.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Project submitted successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var internId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == id && p.InternId == internId);

            if (project == null)
                return NotFound();

            return View(project);
        }

        public async Task<IActionResult> Download(int id)
        {
            var internId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == id && p.InternId == internId);

            if (project == null || string.IsNullOrEmpty(project.FilePath))
                return NotFound();

            var filePath = Path.Combine(_environment.WebRootPath, project.FilePath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/octet-stream", project.FileName);
        }
    }
}