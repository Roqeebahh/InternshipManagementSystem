using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using InternshipManagementSystem.Data;
using InternshipManagementSystem.Models;
using InternshipManagementSystem.Models.ViewModels;

namespace InternshipManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserType == "Intern")
                {
                    var intern = _context.Interns.FirstOrDefault(i => i.Email == model.Email);
                    if (intern != null && BCrypt.Net.BCrypt.Verify(model.Password, intern.PasswordHash))
                    {
                        await SignInUser(intern.InternId.ToString(), intern.FullName, "Intern");
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                else
                {
                    var admin = _context.AdminUsers.FirstOrDefault(a => a.Email == model.Email);
                    if (admin != null && BCrypt.Net.BCrypt.Verify(model.Password, admin.PasswordHash))
                    {
                        await SignInUser(admin.AdminUserId.ToString(), admin.FullName, admin.Role);
                        return RedirectToAction("Index", "Admin");
                    }
                }

                ModelState.AddModelError("", "Invalid login credentials.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_context.Interns.Any(i => i.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(model);
                }

                var intern = new Intern
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Institution = model.Institution,
                    CourseOfStudy = model.CourseOfStudy,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Interns.Add(intern);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(AdminRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_context.AdminUsers.Any(a => a.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(model);
                }

                var admin = new AdminUser
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = model.Role,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.AdminUsers.Add(admin);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Admin registration successful! Please login.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(string userId, string name, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}