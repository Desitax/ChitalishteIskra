using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.TeacherAvailabilities;
using ChitalishteIskra.Models.TeacherAvailabilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChitalishteIskra.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherAvailabilitiesController : Controller
    {
        private readonly ITeacherAvailabilityService service;

        public TeacherAvailabilitiesController(ITeacherAvailabilityService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            Guid teacherId = Guid.Parse(currentUserId);

            var data = await service.GetAllAsync();

            var model = data
                .Where(x => x.TeacherId == teacherId)
                .Select(x => new TeacherAvailabilityIndexViewModel
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

        [HttpGet]
        public IActionResult Create()
        {
            var model = new TeacherAvailabilitiesCreateViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherAvailabilitiesCreateViewModel model)
        {
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(string.Empty, "Крайният час трябва да е след началния.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            Guid teacherId = Guid.Parse(currentUserId);

            var dto = new CreateTeacherAvailabilityDto
            {
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TeacherId = teacherId,
                IsAvailable = model.IsAvailable
            };

            await service.CreateAsync(dto);
            TempData["SuccessMessage"] = "Свободният час беше добавен успешно.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            TeacherAvailabilityDto data;

            try
            {
                data = await service.GetByIdAsync(id);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            Guid teacherId = Guid.Parse(currentUserId);

            if (data.TeacherId != teacherId)
            {
                return Forbid();
            }

            var model = new TeacherAvailabilitiesCreateViewModel
            {
                TeacherId = data.TeacherId,
                DayOfWeek = data.DayOfWeek,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                IsAvailable = data.IsAvailable
            };

            ViewBag.Id = id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TeacherAvailabilitiesCreateViewModel model)
        {
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(string.Empty, "Крайният час трябва да е след началния.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Id = id;
                return View(model);
            }

            TeacherAvailabilityDto existing;

            try
            {
                existing = await service.GetByIdAsync(id);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            Guid teacherId = Guid.Parse(currentUserId);

            if (existing.TeacherId != teacherId)
            {
                return Forbid();
            }

            var dto = new CreateTeacherAvailabilityDto
            {
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TeacherId = teacherId,
                IsAvailable = model.IsAvailable
            };

            await service.UpdateAsync(id, dto);
            TempData["SuccessMessage"] = "Свободният час беше редактиран успешно.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            TeacherAvailabilityDto existing;

            try
            {
                existing = await service.GetByIdAsync(id);
            }
            catch (ArgumentException)
            {
                return RedirectToAction(nameof(Index));
            }

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            Guid teacherId = Guid.Parse(currentUserId);

            if (existing.TeacherId != teacherId)
            {
                return Forbid();
            }

            await service.DeleteAsync(id);
            TempData["SuccessMessage"] = "Свободният час беше изтрит успешно.";

            return RedirectToAction(nameof(Index));
        }
    }
}