using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.BookLessons;
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
        private readonly IBookLessonService bookLessonService;
        public readonly ChitalishteIskraDbContext context;
        private readonly UserManager<User> userManager;
        public BookLessonsController(ChitalishteIskraDbContext _context, UserManager<User> _userManager,
            IBookLessonService _bookLessonService)
        {
            this.context = _context;
            this.userManager = _userManager;
            this.bookLessonService = _bookLessonService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await bookLessonService.GetAllAsync();

            var model = data.Select(b => new BookLessonIndexViewModel
            {
                Id = b.Id,
                Date = b.Date,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                TeacherName = b.TeacherName,
                LessonName = b.LessonName
            });

            return View(model);
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
            var data = await bookLessonService.GetCreatePageDataAsync();

            var model = new BookLessonCreateViewModel
            {
                Lessons = data.Lessons.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                }),
                AvailableSlots = data.AvailableSlots.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                })
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
                var pageData = await bookLessonService.GetCreatePageDataAsync();

                model.Lessons = pageData.Lessons.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });

                model.AvailableSlots = pageData.AvailableSlots.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });

                return View(model);
            }

            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);

            try
            {
                var dto = new CreateBookLessonDto
                {
                    LessonId = model.LessonId,
                    TeacherAvailabilityId = model.TeacherAvailabilityId,
                    StudentId = currentUserId
                };

                await bookLessonService.CreateAsync(dto);

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var pageData = await bookLessonService.GetCreatePageDataAsync();

                model.Lessons = pageData.Lessons.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });

                model.AvailableSlots = pageData.AvailableSlots.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });

                return View(model);
            }
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
