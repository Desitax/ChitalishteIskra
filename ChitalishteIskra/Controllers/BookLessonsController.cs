using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.BookLessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static ChitalishteIskra.Data.Entities.LessonType;

namespace ChitalishteIskra.Controllers
{
    [Authorize]
    public class BookLessonsController : Controller
    {
        public readonly ChitalishteIskraDbContext context;
        private readonly UserManager<User> userManager;
        public BookLessonsController(ChitalishteIskraDbContext _context,
            UserManager<User> _userManager)
        {
            this.context = _context;
            this.userManager = _userManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var teachers = await userManager.GetUsersInRoleAsync("Teacher");
            var teachersIds = teachers.Select(t => t.Id).ToList();

            var booking = await context.BookLessons.Include(b => b.Teacher)
                .Include(b => b.Lesson)
                .Where(x => teachersIds.Contains(x.Teacher.Id))
                .Select(b => new BookLessonIndexViewModel
                {
                    Id = b.Id,
                    Date = b.Date,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    TeacherName = b.Teacher.FirstName + " " + b.Teacher.LastName,
                    LessonName = b.Lesson.Name,

                })
            .ToListAsync();
            return View(booking);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var teachers = await context.Users.ToListAsync();
            ViewBag.Teachers = new SelectList(teachers, "Id", "FirstName");

            var lessons = await context.Lessons.ToListAsync();
            ViewBag.Lessons = new SelectList(lessons, "Id", "Name");

            var model = new BookLessonCreateViewModel
            {
                Teachers = context.Users
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FirstName + " " + u.LastName
            }),

                Lessons = context.Lessons
            .Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Name
            })
            };

            var lessonType = new LessonType();

            ViewBag.LessonTypes = Enum.GetValues(typeof(LessonTypeName))
           .Cast<LessonTypeName>()
           .Select(e => new SelectListItem
           {
               Value = ((int)e).ToString(),
               Text = e.ToString()
           })
           .ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookLessonCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var teachers = await context.Users.ToListAsync();
                ViewBag.Teachers = new SelectList(teachers, "Id", "FirstName");

                var lessons = await context.Lessons.ToListAsync();
                ViewBag.Lessons = new SelectList(lessons, "Id", "Name");

                model.Teachers = context.Users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.FirstName + " " + u.LastName
                }).ToList();

                model.Lessons = context.Lessons.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList();

                ViewBag.LessonTypes = Enum.GetValues(typeof(LessonTypeName))
            .Cast<LessonTypeName>()
            .Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            }).ToList();

                return View(model);
            }

            var booking = new BookLesson
            {
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TeacherId = model.TeacherId,
                LessonId = model.LessonId
            };

            await context.BookLessons.AddAsync(booking);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var teachers = await context.Users.ToListAsync();
            ViewBag.Teachers = new SelectList(teachers, "Id", "FirstName");

            var lessons = await context.Lessons.ToListAsync();
            ViewBag.Lessons = new SelectList(lessons, "Id", "Name");

            var booking = await context.BookLessons.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            var model = new BookLessonEditViewModel
            {
                Id = booking.Id,
                Date = booking.Date,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                TeacherId = booking.TeacherId,
                LessonId = booking.LessonId,

                Teachers = context.Users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.FirstName + " " + u.LastName
                }),

                Lessons = context.Lessons.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookLessonEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var teachers = await context.Users.ToListAsync();
                ViewBag.Teachers = new SelectList(teachers, "Id", "FirstName");

                var lessons = await context.Lessons.ToListAsync();
                ViewBag.Lessons = new SelectList(lessons, "Id", "Name");

                model.Teachers = context.Users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.FirstName + " " + u.LastName
                });

                model.Lessons = context.Lessons.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                });

                return View(model);
            }

            var booking = await context.BookLessons.FindAsync(model.Id);

            if (booking == null)
            {
                return NotFound();
            }

            booking.Date = model.Date;
            booking.StartTime = model.StartTime;
            booking.EndTime = model.EndTime;
            booking.TeacherId = model.TeacherId;
            booking.LessonId = model.LessonId;

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var booking = await context.BookLessons.FirstOrDefaultAsync(b => b.Id == id);
            if (booking != null)
            {
                context.BookLessons.Remove(booking);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
