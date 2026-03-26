using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.TeacherAvailabilities;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.TeacherAvailabilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChitalishteIskra.Controllers
{
    [Authorize]
    public class TeacherAvailabilities:Controller
    {

        private readonly ITeacherAvailabilityService service;

        public TeacherAvailabilities(ITeacherAvailabilityService service)
        {
            this.service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await service.GetAllAsync();

            var model = data.Select(x => new TeacherAvailabilityIndexViewModel
            {
                Id = x.Id,
                TeacherName = x.TeacherName,
                Date = x.Date,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                IsAvailable = x.IsAvailable
            });

            return View(model);
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new TeacherAvailabilitiesCreateViewModel());
        }


        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(TeacherAvailabilitiesCreateViewModel model)
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

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var dto = new CreateTeacherAvailabilityDto
            {
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TeacherId = Guid.Parse(currentUserId)
            };

            await service.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await service.GetByIdAsync(id);

            var model = new TeacherAvailabilitiesCreateViewModel
            {
                Date = data.Date,
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
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            await service.UpdateAsync(id, dto);

            return RedirectToAction(nameof(Index));
        }

    }
}
