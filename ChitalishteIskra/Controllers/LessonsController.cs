using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Lessons;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ChitalishteIskra.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class LessonsController : Controller
    {
        private readonly ILessonService lessonService;

        public LessonsController(ILessonService lessonService)
        {
            this.lessonService = lessonService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<LessonDto> data;

            if (User.IsInRole("Admin"))
            {
                data = await lessonService.GetAllAsync();
            }
            else
            {
                string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(currentUserIdString))
                {
                    return Unauthorized();
                }

                Guid currentUserId = Guid.Parse(currentUserIdString);
                data = await lessonService.GetByTeacherIdAsync(currentUserId);
            }

            var model = data.Select(l => new LessonIndexViewModel
            {
                Id = l.Id,
                Name = l.Name,
                TypeName = l.TypeName
            });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new LessonCreateViewModel();

            if (User.IsInRole("Teacher"))
            {
                string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(currentUserIdString))
                {
                    return Unauthorized();
                }

                Guid currentUserId = Guid.Parse(currentUserIdString);
                await PopulateAssignedLessonsAsync(model, currentUserId);
                PopulateLessonTypes();
            }

            ViewBag.IsAdmin = User.IsInRole("Admin");
            ViewBag.IsTeacher = User.IsInRole("Teacher");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonCreateViewModel model)
        {
            ViewBag.IsAdmin = User.IsInRole("Admin");
            ViewBag.IsTeacher = User.IsInRole("Teacher");

            if (User.IsInRole("Admin"))
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    ModelState.AddModelError(nameof(model.Name), "Въведи име на предмет");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                try
                {
                    await lessonService.CreateAsync(new CreateLessonDto
                    {
                        Name = model.Name
                    });

                    TempData["SuccessMessage"] = "Предметът беше добавен успешно.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }
            }

            if (User.IsInRole("Teacher"))
            {
                string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(currentUserIdString))
                {
                    return Unauthorized();
                }

                Guid currentUserId = Guid.Parse(currentUserIdString);

                if (model.LessonId == Guid.Empty)
                {
                    ModelState.AddModelError(nameof(model.LessonId), "Избери предмет");
                }

                if (string.IsNullOrWhiteSpace(model.TypeName))
                {
                    ModelState.AddModelError(nameof(model.TypeName), "Избери тип");
                }

                if (!ModelState.IsValid)
                {
                    await PopulateAssignedLessonsAsync(model, currentUserId);
                    PopulateLessonTypes();
                    return View(model);
                }

                try
                {
                    await lessonService.CreateForTeacherAsync(new TeacherCreateLessonDto
                    {
                        TeacherId = currentUserId,
                        LessonId = model.LessonId,
                        TypeName = model.TypeName
                    });

                    TempData["SuccessMessage"] = "Типът на предмета беше запазен успешно.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    await PopulateAssignedLessonsAsync(model, currentUserId);
                    PopulateLessonTypes();
                    return View(model);
                }
            }

            return Forbid();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var data = await lessonService.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            var model = new LessonEditViewModel
            {
                Id = data.Id,
                Name = data.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LessonEditViewModel model)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await lessonService.UpdateAsync(model.Id, new CreateLessonDto
                {
                    Name = model.Name
                });

                TempData["SuccessMessage"] = "Предметът беше редактиран успешно.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                await lessonService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Предметът беше изтрит успешно.";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateLessonTypes()
        {
            ViewBag.LessonTypes = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = Lesson.LessonTypeName.Individual.ToString(),
                    Text = "Индивидуален"
                },
                new SelectListItem
                {
                    Value = Lesson.LessonTypeName.Group.ToString(),
                    Text = "Групов"
                }
            };
        }

        private async Task PopulateAssignedLessonsAsync(LessonCreateViewModel model, Guid teacherId)
        {
            var assignedLessons = await lessonService.GetAssignedToTeacherAsync(teacherId);

            model.Lessons = assignedLessons.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Name
            });
        }
    }
}