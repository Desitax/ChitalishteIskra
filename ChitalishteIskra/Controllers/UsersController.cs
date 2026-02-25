using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public UsersController(UserManager<User> _userManager, SignInManager<User> _signInManager,
            RoleManager<IdentityRole<Guid>> _roleManager)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            var child = new User()
            {
                FirstName = model.ChildInfo.FirstName,
                LastName = model.ChildInfo.LastName,
                Age = model.ChildInfo.Age,
            };

            if (!string.IsNullOrEmpty(model.ParentInfo.FirstName))
            {
                var parent = new User()
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FirstName = model.ParentInfo.FirstName,
                    LastName = model.ParentInfo.LastName,
                    Age = model.ParentInfo.Age,
                };

                child.Email = "child_" + model.Email;
                child.UserName = "child" + model.UserName;

                var parentResult = await userManager.CreateAsync(parent, model.Password);
                await userManager.AddToRoleAsync(parent, "Parent");

                if (!parentResult.Succeeded)
                {
                    foreach (var item in parentResult.Errors)
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
            await userManager.AddToRoleAsync(child, "Student");

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Users");
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
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

            ModelState.AddModelError(string.Empty, "Invalid login");
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

    }
}
