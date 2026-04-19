using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.TeacherLessons;
using ChitalishteIskra.Models.TeacherLessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChitalishteIskra.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeacherLessonsController : Controller
    {
        private readonly ITeacherLessonService teacherLessonService;

        public TeacherLessonsController(ITeacherLessonService teacherLessonService)
        {
            this.teacherLessonService = teacherLessonService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await teacherLessonService.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var data = await teacherLessonService.GetCreatePageDataAsync();

            var model = new TeacherLessonCreateViewModel
            {
                Teachers = data.Teachers.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                }),
                Lessons = data.Lessons.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherLessonCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var data = await teacherLessonService.GetCreatePageDataAsync();

                model.Teachers = data.Teachers.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });

                model.Lessons = data.Lessons.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });

                return View(model);
            }

            try
            {
                await teacherLessonService.CreateAsync(new CreateTeacherLessonDto
                {
                    TeacherId = model.TeacherId,
                    LessonId = model.LessonId
                });

                TempData["SuccessMessage"] = "Предметът беше назначен успешно.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var data = await teacherLessonService.GetCreatePageDataAsync();

                model.Teachers = data.Teachers.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });

                model.Lessons = data.Lessons.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });

                return View(model);
            }
        }
    }
}