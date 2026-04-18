using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Groups;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChitalishteIskra.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class GroupsController : Controller
    {
        private readonly IGroupService groupService;
        private readonly UserManager<User> userManager;

        public GroupsController(IGroupService groupService, UserManager<User> userManager)
        {
            this.groupService = groupService;
            this.userManager = userManager;
        }

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
                false,
                true);

            var model = data.Select(g => new GroupIndexViewModel
            {
                Id = g.Id,
                Name = g.Name,
                TeacherId = g.TeacherId,
                TeacherName = g.TeacherName
            });

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new GroupCreateViewModel();
            return View(model);
        }

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
            model.TeacherId = currentUserId;

            if (!ModelState.IsValid)
            {
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
                return View(model);
            }
        }

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

            if (data.TeacherId != currentUserId)
            {
                return Forbid();
            }

            ViewBag.SelectedTeacherName = data.TeacherName;

            var model = new GroupEditViewModel
            {
                Id = data.Id,
                Name = data.Name,
                TeacherId = data.TeacherId
            };

            return View(model);
        }

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

            var existingGroup = await groupService.GetByIdAsync(model.Id);
            if (existingGroup == null)
            {
                return NotFound();
            }

            if (existingGroup.TeacherId != currentUserId)
            {
                return Forbid();
            }

            model.TeacherId = currentUserId;

            if (!ModelState.IsValid)
            {
                ViewBag.SelectedTeacherName = existingGroup.TeacherName;
                return View(model);
            }

            try
            {
                var dto = new CreateGroupDto
                {
                    Name = model.Name,
                    TeacherId = model.TeacherId
                };

                await groupService.UpdateAsync(model.Id, dto, currentUserId, false);
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
                ViewBag.SelectedTeacherName = existingGroup.TeacherName;
                return View(model);
            }
        }

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

            try
            {
                await groupService.DeleteAsync(id, currentUserId, false);
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
    }
}