using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Events;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChitalishteIskra.Controllers
{
    [Authorize]
    public class EventsController:Controller
    {
        private readonly IEventService eventService;

        public EventsController(IEventService eventService)
        {
            this.eventService = eventService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await eventService.GetAllAsync();

            var model = data.Select(e => new EventIndexViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Date = e.Date,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Location = e.Location
            });

            return View(model);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new CreateEventDto
            {
                Name = model.Name,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Location = model.Location
            };

            await eventService.CreateAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await eventService.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound();
            }


            var model = new EventEditViewModel
            {
                Id = data.Id,
                Name = data.Name,
                Date = data.Date,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                Location = data.Location
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EventEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new CreateEventDto
            {
                Name = model.Name,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Location = model.Location
            };

            await eventService.UpdateAsync(model.Id, dto);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await eventService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
