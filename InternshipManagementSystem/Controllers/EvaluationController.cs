using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using InternshipManagementSystem.Data;
using InternshipManagementSystem.Models;
using System.Security.Claims;

namespace InternshipManagementSystem.Controllers
{
    public class EvaluationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvaluationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> Index()
        {
            var evaluations = await _context.Evaluations
                .Include(e => e.Intern)
                .Include(e => e.Supervisor)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();

            return View(evaluations);
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Interns = new SelectList(
                await _context.Interns.Where(i => i.IsActive).ToListAsync(),
                "InternId", "FullName");

            return View();
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPost]
        public async Task<IActionResult> Create(Evaluation model)
        {
            if (ModelState.IsValid)
            {
                model.SupervisorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                model.EvaluationDate = DateTime.Now;

                _context.Evaluations.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Evaluation created successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Interns = new SelectList(
                await _context.Interns.Where(i => i.IsActive).ToListAsync(),
                "InternId", "FullName", model.InternId);

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var evaluation = await _context.Evaluations
                .Include(e => e.Intern)
                .Include(e => e.Supervisor)
                .FirstOrDefaultAsync(e => e.EvaluationId == id);

            if (evaluation == null)
                return NotFound();

            return View(evaluation);
        }


        public async Task<IActionResult> InternEvaluations()
        {
           
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            
            if (!User.IsInRole("Intern"))
            {
                return RedirectToAction("Index", "Home");
            }

            var internIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(internIdString) || !int.TryParse(internIdString, out int internId))
            {
                return RedirectToAction("Login", "Account");
            }

            var evaluations = await _context.Evaluations
                .Include(e => e.Supervisor)
                .Where(e => e.InternId == internId)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();

            return View(evaluations);
        }

    }
}