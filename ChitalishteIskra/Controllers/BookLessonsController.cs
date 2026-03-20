using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Models.BookLessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static ChitalishteIskra.Data.Entities.Lesson;

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

            var booking = await context.BookLessons
                .Include(b => b.Teacher)
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

        //[Authorize(Roles = "Admin")]
        //[HttpGet]
        //public async Task<IActionResult> Create()
        //{
        //    var teachers = await context.Users.ToListAsync();
        //    ViewBag.Teachers = new SelectList(teachers, "Id", "FirstName");

        //    var lessons = await context.Lessons.Include(t=>t.Type)
        //        .Select(l => new
        //    {
        //        l.Id,
        //        Text = l.Name + " (" + (l.Type != null ? l.Type.Name.ToString() : "No type") + ")"
        //    })
        //.ToListAsync();

        //    ViewBag.Lessons = new SelectList(lessons, "Id", "Text");

        //    var model = new BookLessonCreateViewModel
        //    {
        //        Teachers = context.Users
        //    .Select(u => new SelectListItem
        //    {
        //        Value = u.Id.ToString(),
        //        Text = u.FirstName + " " + u.LastName
        //    }),

        //        Lessons = context.Lessons
        //    .Select(l => new SelectListItem
        //    {
        //        Value = l.Id.ToString(),
        //        Text = l.Name
        //    })
        //    };

        //    var lessonType = new LessonType();

        //    ViewBag.LessonTypes = Enum.GetValues(typeof(LessonTypeName))
        //   .Cast<LessonTypeName>()
        //   .Select(e => new SelectListItem
        //   {
        //       Value = ((int)e).ToString(),
        //       Text = e.ToString()
        //   })
        //   .ToList();

        //    return View(model);
        //}


        [Authorize(Roles = "Student,Parent")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var lessons = await context.Lessons
                .Where(l => l.TypeName == LessonTypeName.Individual)
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                })
                .ToListAsync();

            var availableSlots = await context.TeacherAvailabilities
                .Where(x => x.IsAvailable)
                .Include(x => x.Teacher)
                .OrderBy(x => x.Date)
                .ThenBy(x => x.StartTime)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Teacher.FirstName + " " + x.Teacher.LastName
                           + " - " + x.Date.ToString()
                           + " - " + x.StartTime.ToString()
                           + " - " + x.EndTime.ToString()
                })
                .ToListAsync();

            var model = new BookLessonCreateViewModel
            {
                Lessons = lessons,
                AvailableSlots = availableSlots
            };

            return View(model);
        }


        //[HttpPost]
        //public async Task<IActionResult> Create(BookLessonCreateViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var teachers = await context.Users.ToListAsync();
        //        ViewBag.Teachers = new SelectList(teachers, "Id", "FirstName");

        //        var lessons = await context.Lessons.ToListAsync();
        //        ViewBag.Lessons = new SelectList(lessons, "Id", "Name");

        //        model.Teachers = context.Users.Select(u => new SelectListItem
        //        {
        //            Value = u.Id.ToString(),
        //            Text = u.FirstName + " " + u.LastName
        //        }).ToList();

        //        model.Lessons = context.Lessons.Select(l => new SelectListItem
        //        {
        //            Value = l.Id.ToString(),
        //            Text = l.Name
        //        }).ToList();

        //        ViewBag.LessonTypes = Enum.GetValues(typeof(LessonTypeName))
        //    .Cast<LessonTypeName>()
        //    .Select(e => new SelectListItem
        //    {
        //        Value = ((int)e).ToString(),
        //        Text = e.ToString()
        //    }).ToList();

        //        return View(model);
        //    }

        //    var booking = new BookLesson
        //    {
        //        Date = model.Date,
        //        StartTime = model.StartTime,
        //        EndTime = model.EndTime,
        //        TeacherId = model.TeacherId,
        //        LessonId = model.LessonId
        //    };

        //    await context.BookLessons.AddAsync(booking);
        //    await context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}

        [Authorize(Roles = "Student,Parent")]
        [HttpPost]
        public async Task<IActionResult> Create(BookLessonCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Lessons = await context.Lessons
                    .Where(l => l.TypeName == LessonTypeName.Individual)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.Name
                    })
                    .ToListAsync();

                model.AvailableSlots = await context.TeacherAvailabilities
                    .Where(x => x.IsAvailable)
                    .Include(x => x.Teacher)
                    .OrderBy(x => x.Date)
                    .ThenBy(x => x.StartTime)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Teacher.FirstName + " " + x.Teacher.LastName
                               + " - " + x.Date.ToString()
                               + " - " + x.StartTime.ToString()
                               + " - " + x.EndTime.ToString()
                    })
                    .ToListAsync();

                return View(model);
            }

            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);

            var slot = await context.TeacherAvailabilities
                .FirstOrDefaultAsync(x => x.Id == model.TeacherAvailabilityId && x.IsAvailable);

            if (slot == null)
            {
                ModelState.AddModelError(string.Empty, "Избраният час вече не е свободен.");

                model.Lessons = await context.Lessons
                    .Where(l => l.TypeName == LessonTypeName.Individual)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.Name
                    })
                    .ToListAsync();

                model.AvailableSlots = await context.TeacherAvailabilities
                    .Where(x => x.IsAvailable)
                    .Include(x => x.Teacher)
                    .OrderBy(x => x.Date)
                    .ThenBy(x => x.StartTime)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Teacher.FirstName + " " + x.Teacher.LastName
                               + " - " + x.Date.ToString()
                               + " - " + x.StartTime.ToString()
                               + " - " + x.EndTime.ToString()
                    })
                    .ToListAsync();

                return View(model);
            }

            var lesson = await context.Lessons.FirstOrDefaultAsync(l => l.Id == model.LessonId);

            if (lesson == null || lesson.TypeName != LessonTypeName.Individual)
            {
                ModelState.AddModelError(string.Empty, "Невалиден урок.");

                model.Lessons = await context.Lessons
                    .Where(l => l.TypeName == LessonTypeName.Individual)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.Name
                    })
                    .ToListAsync();

                model.AvailableSlots = await context.TeacherAvailabilities
                    .Where(x => x.IsAvailable)
                    .Include(x => x.Teacher)
                    .OrderBy(x => x.Date)
                    .ThenBy(x => x.StartTime)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Teacher.FirstName + " " + x.Teacher.LastName
                               + " - " + x.Date.ToString()
                               + " - " + x.StartTime.ToString()
                               + " - " + x.EndTime.ToString()
                    })
                    .ToListAsync();

                return View(model);
            }

            var booking = new BookLesson
            {
                Id = Guid.NewGuid(),
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                TeacherId = slot.TeacherId,
                LessonId = model.LessonId,
                StudentId = currentUserId
            };

            slot.IsAvailable = false;

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



        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> CreateGroup()
        {
            var model = new BookLessonCreateGroupViewModel
            {
                Lessons = await context.Lessons
                    .Where(l => l.TypeName == LessonTypeName.Group)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.Name
                    })
                    .ToListAsync(),

                Groups = await context.Groups
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(BookLessonCreateGroupViewModel model)
        {
            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(string.Empty, "Крайният час трябва да е след началния.");
            }

            var lesson = await context.Lessons.FirstOrDefaultAsync(l => l.Id == model.LessonId);
            if (lesson == null || lesson.TypeName != LessonTypeName.Group)
            {
                ModelState.AddModelError(string.Empty, "Избраният урок не е групов.");
            }

            var groupExists = await context.Groups.AnyAsync(g => g.Id == model.GroupId);
            if (!groupExists)
            {
                ModelState.AddModelError(string.Empty, "Избраната група не съществува.");
            }

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            Guid teacherId = Guid.Parse(currentUserId);

            bool hasConflict = await context.BookLessons.AnyAsync(bl =>
                bl.TeacherId == teacherId &&
                bl.Date == model.Date &&
                bl.StartTime == model.StartTime &&
                bl.EndTime == model.EndTime);

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "Вече има урок за този час.");
            }

            if (!ModelState.IsValid)
            {
                model.Lessons = await context.Lessons
                    .Where(l => l.TypeName == LessonTypeName.Group)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.Name
                    })
                    .ToListAsync();

                model.Groups = await context.Groups
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    })
                    .ToListAsync();

                return View(model);
            }

            var booking = new BookLesson
            {
                Id = Guid.NewGuid(),
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TeacherId = teacherId,
                LessonId = model.LessonId,
                GroupId = model.GroupId,
                StudentId = null
            };

            await context.BookLessons.AddAsync(booking);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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

        private string GetCurrentClientId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
