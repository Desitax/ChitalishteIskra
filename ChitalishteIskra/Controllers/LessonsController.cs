using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Lessons;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static ChitalishteIskra.Data.Entities.Lesson;

namespace ChitalishteIskra.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LessonsController:Controller
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
            ViewBag.LessonTypes = Enum.GetValues(typeof(Lesson.LessonTypeName))
                .Cast<Lesson.LessonTypeName>()
                .Select(d => new SelectListItem
                {
                    Text = d.ToString(),
                    Value = d.ToString()
                })
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LessonCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LessonTypes = Enum.GetValues(typeof(Lesson.LessonTypeName))
                    .Cast<Lesson.LessonTypeName>()
                    .Select(d => new SelectListItem
                    {
                        Text = d.ToString(),
                        Value = d.ToString()
                    })
                    .ToList();

                return View(model);
            }

            var dto = new CreateLessonDto
            {
                Name = model.Name,
                TypeName = model.TypeName
            };

            await lessonService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await lessonService.GetByIdAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            ViewBag.LessonTypes = Enum.GetValues(typeof(Lesson.LessonTypeName))
                .Cast<Lesson.LessonTypeName>()
                .Select(d => new SelectListItem
                {
                    Text = d.ToString(),
                    Value = d.ToString()
                })
                .ToList();

            var model = new LessonEditViewModel
            {
                Id = data.Id,
                Name = data.Name,
                TypeName = data.TypeName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LessonEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LessonTypes = Enum.GetValues(typeof(Lesson.LessonTypeName))
                    .Cast<Lesson.LessonTypeName>()
                    .Select(d => new SelectListItem
                    {
                        Text = d.ToString(),
                        Value = d.ToString()
                    })
                    .ToList();

                return View(model);
            }

            var dto = new CreateLessonDto
            {
                Name = model.Name,
                TypeName = model.TypeName
            };

            await lessonService.UpdateAsync(model.Id, dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await lessonService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
