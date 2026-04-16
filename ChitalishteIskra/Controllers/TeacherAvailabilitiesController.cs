using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.TeacherAvailabilities;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.TeacherAvailabilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChitalishteIskra.Controllers
{
    [Authorize]
    public class TeacherAvailabilitiesController : Controller
    {
        private readonly ITeacherAvailabilityService service;
        private readonly UserManager<User> userManager;

        public TeacherAvailabilitiesController(
            ITeacherAvailabilityService service,
            UserManager<User> userManager)
        {
            this.service = service;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await service.GetAllAsync();

            var model = data.Select(x => new TeacherAvailabilityIndexViewModel
            {
                Id = x.Id,
                TeacherName = x.TeacherName,
                DayOfWeek = x.DayOfWeek,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                IsAvailable = x.IsAvailable
            });

            return View(model);
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var teachers = await userManager.GetUsersInRoleAsync("Teacher");

            ViewBag.Teachers = teachers.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.FirstName + " " + t.LastName
            }).ToList();

            var model = new TeacherAvailabilitiesCreateViewModel();

            return View(model);
        }


        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(TeacherAvailabilitiesCreateViewModel model)
        {
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(string.Empty, "Крайният час трябва да е след началния.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                var teachers = await userManager.GetUsersInRoleAsync("Teacher");

                ViewBag.Teachers = teachers.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.FirstName + " " + t.LastName
                }).ToList();

                return View(model);
            }

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            Guid teacherId;

            if (User.IsInRole("Admin"))
            {
                teacherId = model.TeacherId;
            }
            else
            {
                teacherId = Guid.Parse(currentUserId);
            }

            var dto = new CreateTeacherAvailabilityDto
            {
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TeacherId = teacherId
            };

            await service.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Teacher,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await service.GetByIdAsync(id);

            var model = new TeacherAvailabilitiesCreateViewModel
            {
                DayOfWeek = data.DayOfWeek,
                StartTime = data.StartTime,
                EndTime = data.EndTime
            };

            ViewBag.Id = id;

            return View(model);
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, TeacherAvailabilitiesCreateViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(string.Empty, "Крайният час трябва да е след началния.");
                return View(model);
            }

            var dto = new CreateTeacherAvailabilityDto
            {
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            await service.UpdateAsync(id, dto);

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
