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

            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent(RegisterViewModel model)
        {
            bool hasParentInfo =
                !string.IsNullOrWhiteSpace(model.ParentInfo.FirstName) &&
                !string.IsNullOrWhiteSpace(model.ParentInfo.LastName) &&
                model.ParentInfo.Age.HasValue;

            if (model.ChildInfo.Age < 8 && !hasParentInfo)
            {
                ModelState.AddModelError(string.Empty, "Трябва да се регистрирате с родител.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var child = new User
            {
                FirstName = model.ChildInfo.FirstName,
                LastName = model.ChildInfo.LastName,
                Age = model.ChildInfo.Age
            };

            if (hasParentInfo)
            {
                string parentFirstName = model.ParentInfo.FirstName!;
                string parentLastName = model.ParentInfo.LastName!;
                int parentAge = model.ParentInfo.Age!.Value;

                var parent = new User
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FirstName = parentFirstName,
                    LastName = parentLastName,
                    Age = parentAge
                };

                child.Email = "child_" + model.Email;
                child.UserName = "child_" + model.UserName;

                var parentResult = await userManager.CreateAsync(parent, model.Password);

                if (!parentResult.Succeeded)
                {
                    foreach (var item in parentResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }

                    return View(model);
                }

                var parentRoleResult = await userManager.AddToRoleAsync(parent, "Parent");
                if (!parentRoleResult.Succeeded)
                {
                    foreach (var item in parentRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }

                    return View(model);
                }
            }
            else
            {
                child.Email = model.Email;
                child.UserName = model.UserName;
            }

            var result = await userManager.CreateAsync(child, model.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View(model);
            }

            var childRoleResult = await userManager.AddToRoleAsync(child, "Student");
            if (!childRoleResult.Succeeded)
            {
                foreach (var item in childRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View(model);
            }

            await signInManager.SignInAsync(child, isPersistent: false);

            return RedirectToAction("Login", "Users", new
            {
                username = child.UserName,
                password = model.Password
            });
        }

        [HttpGet]
        public IActionResult Login(string? username, string? password)
        {
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                if (user.IsTeacherRequest == true && user.IsApprovedTeacher == false)
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
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> SeedRoles()
        {
            string[] roles = { "Admin", "Teacher", "Parent", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            return Content("Roles seeded (created if missing).");
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
        public async Task<IActionResult> RegisterTeacher(TeacherRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
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
    }
}