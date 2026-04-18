using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Lessons;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var data = await lessonService.GetAllAsync();

            var model = data.Select(l => new LessonIndexViewModel
            {
                Id = l.Id,
                Name = l.Name,
                TypeName = l.TypeName
            });

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateLessonTypes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateLessonTypes();
                return View(model);
            }

            try
            {
                var dto = new CreateLessonDto
                {
                    Name = model.Name,
                    TypeName = model.TypeName
                };

                await lessonService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Предметът беше добавен успешно.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateLessonTypes();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await lessonService.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            PopulateLessonTypes();

            var model = new LessonEditViewModel
            {
                Id = data.Id,
                Name = data.Name,
                TypeName = data.TypeName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LessonEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateLessonTypes();
                return View(model);
            }

            try
            {
                var dto = new CreateLessonDto
                {
                    Name = model.Name,
                    TypeName = model.TypeName
                };

                await lessonService.UpdateAsync(model.Id, dto);
                TempData["SuccessMessage"] = "Предметът беше редактиран успешно.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateLessonTypes();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
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
    }
}