using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InternshipManagementSystem.Data;
using InternshipManagementSystem.Models;
using System.Security.Claims;

namespace InternshipManagementSystem.Controllers
{
    [Authorize(Roles = "Intern")]
    public class LogbookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogbookController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var internId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var logs = await _context.DailyLogs
                .Where(d => d.InternId == internId)
                .OrderByDescending(d => d.LogDate)
                .ToListAsync();

            return View(logs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DailyLog model)
        {
            if (ModelState.IsValid)
            {
                model.InternId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                model.CreatedAt = DateTime.Now;

                _context.DailyLogs.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Daily log entry added successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var internId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var log = await _context.DailyLogs
                .FirstOrDefaultAsync(d => d.DailyLogId == id && d.InternId == internId);

            if (log == null)
                return NotFound();

            return View(log);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DailyLog model)
        {
            if (ModelState.IsValid)
            {
                var internId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var log = await _context.DailyLogs
                    .FirstOrDefaultAsync(d => d.DailyLogId == model.DailyLogId && d.InternId == internId);

                if (log == null)
                    return NotFound();

                log.LogDate = model.LogDate;
                log.Activity = model.Activity;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Daily log updated successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var internId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var log = await _context.DailyLogs
                .FirstOrDefaultAsync(d => d.DailyLogId == id && d.InternId == internId);

            if (log != null)
            {
                _context.DailyLogs.Remove(log);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Daily log deleted successfully!";
            }

            return RedirectToAction("Index");
        }
    }
}