using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Events;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChitalishteIskra.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IEventService eventService;
        private readonly IEmailSender emailSender;
        private readonly UserManager<User> userManager;
        private readonly IImageService imageService;

        public EventsController(
            IEventService eventService,
            IEmailSender emailSender,
            UserManager<User> userManager,
            IImageService imageService)
        {
            this.eventService = eventService;
            this.emailSender = emailSender;
            this.userManager = userManager;
            this.imageService = imageService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchName,
            string? searchDatePart,
            string? searchDateType,
            string? searchLocation,
            string? searchStatus)
        {
            var data = await eventService.GetAllAsync();

            var allEvents = data.Select(e => new EventIndexViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Date = e.Date,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Location = e.Location,
                Description = e.Description,
                ImageUrl = e.ImageUrl
            }).ToList();

            IEnumerable<EventIndexViewModel> filteredEvents = allEvents;
            var now = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                filteredEvents = allEvents.Where(e =>
                    !string.IsNullOrWhiteSpace(e.Name) &&
                    e.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase));
            }
            else if (!string.IsNullOrWhiteSpace(searchDateType) &&
                     !string.IsNullOrWhiteSpace(searchDatePart) &&
                     int.TryParse(searchDatePart, out int dateNumber))
            {
                switch (searchDateType.ToLower())
                {
                    case "day":
                        if (dateNumber >= 1 && dateNumber <= 31)
                        {
                            filteredEvents = allEvents.Where(e => e.Date.Day == dateNumber);
                        }
                        else
                        {
                            filteredEvents = allEvents;
                        }
                        break;

                    case "month":
                        if (dateNumber >= 1 && dateNumber <= 12)
                        {
                            filteredEvents = allEvents.Where(e => e.Date.Month == dateNumber);
                        }
                        else
                        {
                            filteredEvents = allEvents;
                        }
                        break;

                    case "year":
                        if (dateNumber >= 1)
                        {
                            filteredEvents = allEvents.Where(e => e.Date.Year == dateNumber);
                        }
                        else
                        {
                            filteredEvents = allEvents;
                        }
                        break;

                    default:
                        filteredEvents = allEvents;
                        break;
                }
            }
            else if (!string.IsNullOrWhiteSpace(searchLocation))
            {
                filteredEvents = allEvents.Where(e =>
                    !string.IsNullOrWhiteSpace(e.Location) &&
                    e.Location.Contains(searchLocation, StringComparison.OrdinalIgnoreCase));
            }
            else if (!string.IsNullOrWhiteSpace(searchStatus))
            {
                switch (searchStatus.ToLower())
                {
                    case "predstoqshti":
                        filteredEvents = allEvents.Where(e =>
                            e.Date.ToDateTime(e.EndTime) >= now);
                        break;

                    case "izminali":
                        filteredEvents = allEvents.Where(e =>
                            e.Date.ToDateTime(e.EndTime) < now);
                        break;

                    default:
                        filteredEvents = allEvents;
                        break;
                }
            }

            var model = new EventsListViewModel
            {
                Events = filteredEvents
                    .OrderBy(e => e.Date)
                    .ThenBy(e => e.StartTime)
                    .ToList(),

                SearchName = searchName,
                SearchDatePart = searchDatePart,
                SearchDateType = searchDateType,
                SearchLocation = searchLocation,
                SearchStatus = searchStatus,

                NameSuggestions = allEvents
                    .Select(e => e.Name)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),

                LocationSuggestions = allEvents
                    .Select(e => e.Location)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var data = await eventService.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            var model = new EventIndexViewModel
            {
                Id = data.Id,
                Name = data.Name,
                Date = data.Date,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                Location = data.Location,
                Description = data.Description,
                ImageUrl = data.ImageUrl
            };

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
        [ValidateAntiForgeryToken]
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

            if (model.ImageFile == null)
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Снимката е задължителна.");
            }
            else if (!model.ImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Качи валидна снимка.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string fileName = $"events_{Guid.NewGuid()}_{SanitizeFileName(model.Name)}";
            string? imageUrl = await imageService.UploadImageAsync(model.ImageFile!, fileName);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Снимката не можа да бъде качена в Cloudinary.");
                return View(model);
            }

            var dto = new CreateEventDto
            {
                Name = model.Name,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Location = model.Location,
                Description = model.Description,
                ImageUrl = imageUrl
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

            string subject = $"Ново събитие: {model.Name}";
            string message = $@"
                <h2>📅 Предстоящо събитие</h2>
                <p><strong>Име:</strong> {model.Name}</p>
                <p><strong>Дата:</strong> {model.Date:dd.MM.yyyy}</p>
                <p><strong>Начало:</strong> {model.StartTime:HH\:mm}</p>
                <p><strong>Край:</strong> {model.EndTime:HH\:mm}</p>
                <p><strong>Място:</strong> {model.Location}</p>
                <p><strong>Описание:</strong> {model.Description}</p>
                <p><strong>Подробности:</strong> посетете сайта на читалището.</p>
                <br />
                <p>Очакваме Ви!</p>";

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

            TempData["SuccessMessage"] = "Събитието е създадено успешно и известията са изпратени.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Teacher")]
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
                Location = data.Location,
                Description = data.Description,
                ExistingImageUrl = data.ImageUrl
            };

            return View(model);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            if (model.ImageFile != null && !model.ImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Качи валидна снимка.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? finalImageUrl = model.ExistingImageUrl;

            if (model.ImageFile != null)
            {
                string fileName = $"events_{Guid.NewGuid()}_{SanitizeFileName(model.Name)}";
                string? uploadedImageUrl = await imageService.UploadImageAsync(model.ImageFile, fileName);

                if (string.IsNullOrWhiteSpace(uploadedImageUrl))
                {
                    ModelState.AddModelError(nameof(model.ImageFile), "Снимката не можа да бъде качена в Cloudinary.");
                    return View(model);
                }

                finalImageUrl = uploadedImageUrl;
            }

            var dto = new CreateEventDto
            {
                Name = model.Name,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Location = model.Location,
                Description = model.Description,
                ImageUrl = finalImageUrl
            };

            await eventService.UpdateAsync(model.Id, dto);

            TempData["SuccessMessage"] = "Събитието е редактирано успешно.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await eventService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Събитието е изтрито успешно.";
            return RedirectToAction(nameof(Index));
        }

        private static string SanitizeFileName(string input)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var clean = new string(input.Where(ch => !invalidChars.Contains(ch)).ToArray());

            return string.IsNullOrWhiteSpace(clean)
                ? "event-image"
                : clean.Replace(' ', '-').ToLowerInvariant();
        }
    }
}