using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models;
using ChitalishteIskra.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public UsersController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public IActionResult RegisterStudent()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudent(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!await roleManager.RoleExistsAsync("Student") ||
                !await roleManager.RoleExistsAsync("Teacher") ||
                !await roleManager.RoleExistsAsync("Admin"))
            {
                await SeedRolesInternal();
            }

            var existingUserByName = await userManager.FindByNameAsync(model.UserName);
            if (existingUserByName != null)
            {
                ModelState.AddModelError(string.Empty, "Потребителското име вече съществува.");
                return View(model);
            }

            var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Вече има регистриран потребител с този имейл.");
                return View(model);
            }

            var student = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age
            };

            var result = await userManager.CreateAsync(student, model.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View(model);
            }

            var roleResult = await userManager.AddToRoleAsync(student, "Student");

            if (!roleResult.Succeeded)
            {
                foreach (var item in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View(model);
            }

            return RedirectToAction(nameof(Login), new
            {
                username = student.UserName
            });
        }

        [HttpGet]
        public IActionResult Login(string? username, string? password)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel();

            if (!string.IsNullOrEmpty(username))
            {
                model.UserName = username;
            }

            if (!string.IsNullOrEmpty(password))
            {
                model.Password = password;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                if (user.IsTeacherRequest && !user.IsApprovedTeacher)
                {
                    ModelState.AddModelError(string.Empty, "Профилът на учителя все още не е одобрен. Свържете се с администратор.");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    false,
                    false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Невалидно потребителско име или парола.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> SeedRoles()
        {
            await SeedRolesInternal();
            return Content("Roles seeded.");
        }

        [AllowAnonymous]
        public async Task<IActionResult> FixStudent(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return Content("Липсва username.");
            }

            await SeedRolesInternal();

            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Content("Потребителят не е намерен.");
            }

            var roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains("Student"))
            {
                var addResult = await userManager.AddToRoleAsync(user, "Student");

                if (!addResult.Succeeded)
                {
                    return Content("Неуспешно добавяне на роля Student.");
                }
            }

            return Content($"Потребителят {username} вече има роля Student.");
        }

        [HttpGet]
        public IActionResult RegisterTeacher()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new TeacherRegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterTeacher(TeacherRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!await roleManager.RoleExistsAsync("Teacher"))
            {
                await SeedRolesInternal();
            }

            var existingUserByName = await userManager.FindByNameAsync(model.UserName);
            if (existingUserByName != null)
            {
                ModelState.AddModelError(string.Empty, "Потребителското име вече съществува.");
                return View(model);
            }

            var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Вече има регистриран потребител с този имейл.");
                return View(model);
            }

            var teacher = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                IsTeacherRequest = true,
                IsApprovedTeacher = false
            };

            var result = await userManager.CreateAsync(teacher, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            return RedirectToAction(nameof(Login));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> PendingTeachers()
        {
            var users = await userManager.Users
                .Where(u => u.IsTeacherRequest && !u.IsApprovedTeacher)
                .ToListAsync();

            return View(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveTeacher(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            user.IsApprovedTeacher = true;
            user.IsTeacherRequest = false;

            await userManager.UpdateAsync(user);

            if (!await userManager.IsInRoleAsync(user, "Teacher"))
            {
                await userManager.AddToRoleAsync(user, "Teacher");
            }

            return RedirectToAction(nameof(PendingTeachers));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTeacherRequest(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            user.IsTeacherRequest = false;
            user.IsApprovedTeacher = false;

            await userManager.UpdateAsync(user);

            if (await userManager.IsInRoleAsync(user, "Teacher"))
            {
                await userManager.RemoveFromRoleAsync(user, "Teacher");
            }

            return RedirectToAction(nameof(PendingTeachers));
        }

        private async Task SeedRolesInternal()
        {
            string[] roles = { "Admin", "Teacher", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }
    }
}