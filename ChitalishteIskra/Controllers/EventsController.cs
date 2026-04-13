using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Events;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ChitalishteIskra.Controllers
{
    [Authorize]
    public class EventsController:Controller
    {
        private readonly IEventService eventService;
        private readonly IEmailSender emailSender;
        private readonly UserManager<User> userManager;

        public EventsController(IEventService eventService,IEmailSender emailSender,UserManager<User> userManager)
        {
            this.eventService = eventService;
            this.emailSender = emailSender;
            this.userManager = userManager;
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
            if (model.Date < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(nameof(model.Date), "Не може да избирате минала дата.");
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(nameof(model.EndTime), "Крайният час трябва да е след началния час.");
            }

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

            var teachers = await userManager.GetUsersInRoleAsync("Teacher");
            var students = await userManager.GetUsersInRoleAsync("Student");
            var parents = await userManager.GetUsersInRoleAsync("Parent");

            var recipients = teachers
                .Concat(students)
                .Concat(parents)
                .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();
            //TempData["WarningMessage"] = $"Teacher: {teachers.Count}, Student: {students.Count}," +
            //    $" Parent: {parents.Count}, Получатели с email: {recipients.Count}";

            string subject = $"Ново събитие: {model.Name}";

            string message = $@"
            <h2>📅 Предстоящо събитие</h2>
            <p><strong>Име:</strong> {model.Name}</p>
            <p><strong>Дата:</strong> {model.Date}</p>
            <p><strong>Начало:</strong> {model.StartTime}</p>
            <p><strong>Край:</strong> {model.EndTime}</p>
            <p><strong>Място:</strong> {model.Location}</p>
            <br/>
            <p>Очакваме Ви!</p>
            ";

            foreach (var user in recipients)
            {
                try
                {
                    await emailSender.SendEmailAsync(user.Email!, subject, message);
                }
                catch
                {
                    
                }
            }

            TempData["SuccessMessage"] = "Имейлите за предстоящото събитие са изпратени успешно.";

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
            if (model.Date < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(nameof(model.Date), "Не може да избирате минала дата.");
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(nameof(model.EndTime), "Крайният час трябва да е след началния час.");
            }

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
