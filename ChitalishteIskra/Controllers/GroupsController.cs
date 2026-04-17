using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Groups;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ChitalishteIskra.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly IGroupService groupService;
        private readonly UserManager<User> userManager;

        public GroupsController(IGroupService groupService, UserManager<User> userManager)
        {
            this.groupService = groupService;
            this.userManager = userManager;
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);

            var data = await groupService.GetAllAsync(
                currentUserId,
                User.IsInRole("Admin"),
                User.IsInRole("Teacher"));

            var model = data.Select(g => new GroupIndexViewModel
            {
                Id = g.Id,
                Name = g.Name,
                TeacherId = g.TeacherId,
                TeacherName = g.TeacherName
            });

            return View(model);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new GroupCreateViewModel();

            if (User.IsInRole("Admin"))
            {
                await PopulateTeachersInViewBagAsync();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroupCreateViewModel model)
        {
            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);
            bool isAdmin = User.IsInRole("Admin");
            bool isTeacher = User.IsInRole("Teacher");

            if (!isAdmin && !isTeacher)
            {
                return Forbid();
            }

            if (isTeacher)
            {
                model.TeacherId = currentUserId;
            }

            if (!ModelState.IsValid)
            {
                if (isAdmin)
                {
                    await PopulateTeachersInViewBagAsync();
                }

                return View(model);
            }

            try
            {
                var dto = new CreateGroupDto
                {
                    Name = model.Name,
                    TeacherId = model.TeacherId
                };

                await groupService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Групата беше създадена успешно.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                if (isAdmin)
                {
                    await PopulateTeachersInViewBagAsync();
                }

                return View(model);
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await groupService.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);

            if (User.IsInRole("Teacher") && data.TeacherId != currentUserId)
            {
                return Forbid();
            }

            if (User.IsInRole("Admin"))
            {
                await PopulateTeachersInViewBagAsync();
            }
            else
            {
                ViewBag.SelectedTeacherName = data.TeacherName;
            }

            var model = new GroupEditViewModel
            {
                Id = data.Id,
                Name = data.Name,
                TeacherId = data.TeacherId
            };

            return View(model);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GroupEditViewModel model)
        {
            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);
            bool isAdmin = User.IsInRole("Admin");
            bool isTeacher = User.IsInRole("Teacher");

            var existingGroup = await groupService.GetByIdAsync(model.Id);
            if (existingGroup == null)
            {
                return NotFound();
            }

            if (isTeacher && existingGroup.TeacherId != currentUserId)
            {
                return Forbid();
            }

            if (isTeacher)
            {
                model.TeacherId = currentUserId;
            }

            if (!ModelState.IsValid)
            {
                if (isAdmin)
                {
                    await PopulateTeachersInViewBagAsync();
                }
                else
                {
                    ViewBag.SelectedTeacherName = existingGroup.TeacherName;
                }

                return View(model);
            }

            try
            {
                var dto = new CreateGroupDto
                {
                    Name = model.Name,
                    TeacherId = model.TeacherId
                };

                await groupService.UpdateAsync(model.Id, dto, currentUserId, isAdmin);
                TempData["SuccessMessage"] = "Групата беше редактирана успешно.";
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                if (isAdmin)
                {
                    await PopulateTeachersInViewBagAsync();
                }
                else
                {
                    ViewBag.SelectedTeacherName = existingGroup.TeacherName;
                }

                return View(model);
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);
            bool isAdmin = User.IsInRole("Admin");

            try
            {
                await groupService.DeleteAsync(id, currentUserId, isAdmin);
                TempData["SuccessMessage"] = "Групата беше изтрита успешно.";
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateTeachersInViewBagAsync()
        {
            var teachers = await userManager.GetUsersInRoleAsync("Teacher");

            ViewBag.Teachers = teachers
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.FirstName + " " + t.LastName
                })
                .ToList();
        }
    }
}