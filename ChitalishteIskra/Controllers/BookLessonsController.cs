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

        public BookLessonsController(
            ChitalishteIskraDbContext context,
            UserManager<User> userManager,
            IBookLessonService bookLessonService)
        {
            this.context = context;
            this.userManager = userManager;
            this.bookLessonService = bookLessonService;
        }

        [Authorize(Roles = "Admin,Parent,Student,Teacher")]
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

        [Authorize(Roles = "Admin,Student,Parent")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var data = await bookLessonService.GetCreatePageDataAsync();

            var model = new BookLessonCreateViewModel
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                Teachers = data.Teachers.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                })
            };

            return View(model);
        }

        [Authorize(Roles = "Admin,Student,Parent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookLessonCreateViewModel model)
        {
            if (model.Date < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(nameof(model.Date), "Не може да избереш минала дата.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateCreateViewModelAsync(model);
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
                    TeacherId = model.TeacherId,
                    LessonId = model.LessonId,
                    TeacherAvailabilityId = model.TeacherAvailabilityId,
                    StudentId = currentUserId,
                    Date = model.Date
                };

                await bookLessonService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Индивидуалният урок беше записан успешно.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateCreateViewModelAsync(model);
                return View(model);
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> CreateGroup()
        {
            Guid? teacherId = await ResolveTeacherIdForGroupActionsAsync();
            if (teacherId == null)
            {
                return Unauthorized();
            }

            var model = new BookLessonCreateGroupViewModel
            {
                Date = DateOnly.FromDateTime(DateTime.Today)
            };

            await PopulateCreateGroupViewModelAsync(model, teacherId.Value);
            return View(model);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(BookLessonCreateGroupViewModel model)
        {
            Guid? teacherId = await ResolveTeacherIdForGroupActionsAsync();
            if (teacherId == null)
            {
                return Unauthorized();
            }

            if (model.Date < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(nameof(model.Date), "Не може да избереш минала дата.");
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(nameof(model.EndTime), "Крайният час трябва да е след началния.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateCreateGroupViewModelAsync(model, teacherId.Value);
                return View(model);
            }

            try
            {
                var dto = new CreateGroupLessonDto
                {
                    TeacherId = teacherId.Value,
                    LessonId = model.LessonId,
                    GroupId = model.GroupId,
                    Date = model.Date,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                };

                await bookLessonService.CreateGroupAsync(dto);
                TempData["SuccessMessage"] = "Груповият урок беше създаден успешно.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateCreateGroupViewModelAsync(model, teacherId.Value);
                return View(model);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> GroupInvitations()
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var model = await bookLessonService.GetStudentGroupLessonsAsync(Guid.Parse(currentUserId));
            return View(model);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptGroupLesson(Guid bookLessonId)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            try
            {
                await bookLessonService.RespondToGroupLessonAsync(bookLessonId, Guid.Parse(currentUserId), true);
                TempData["SuccessMessage"] = "Поканата беше приета.";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(GroupInvitations));
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineGroupLesson(Guid bookLessonId)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            try
            {
                await bookLessonService.RespondToGroupLessonAsync(bookLessonId, Guid.Parse(currentUserId), false);
                TempData["SuccessMessage"] = "Поканата беше отказана.";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(GroupInvitations));
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
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Student,Parent")]
        [HttpGet]
        public async Task<IActionResult> GetTeacherBookingData(Guid teacherId, DateOnly date)
        {
            var data = await bookLessonService.GetTeacherBookingDataAsync(teacherId, date);

            return Json(new
            {
                lessons = data.Lessons.Select(x => new { value = x.Value, text = x.Text }),
                groups = data.Groups.Select(x => x.Text),
                workingHours = data.WorkingHours,
                availableSlots = data.AvailableSlots.Select(x => new { value = x.Value, text = x.Text })
            });
        }

        private async Task PopulateCreateViewModelAsync(BookLessonCreateViewModel model)
        {
            var pageData = await bookLessonService.GetCreatePageDataAsync();
            model.Teachers = pageData.Teachers.Select(x => new SelectListItem
            {
                Value = x.Value,
                Text = x.Text
            });

            if (model.TeacherId != Guid.Empty)
            {
                var teacherData = await bookLessonService.GetTeacherBookingDataAsync(model.TeacherId, model.Date);
                model.Lessons = teacherData.Lessons.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });
                model.AvailableSlots = teacherData.AvailableSlots.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text
                });
                model.TeacherGroups = teacherData.Groups.Select(g => g.Text);
                model.WorkingHours = teacherData.WorkingHours;
            }
        }

        private async Task PopulateCreateGroupViewModelAsync(BookLessonCreateGroupViewModel model, Guid teacherId)
        {
            model.Lessons = await context.TeacherLessons
                .Where(tl => tl.TeacherId == teacherId && tl.Lesson.TypeName == LessonTypeName.Group)
                .Select(tl => new { tl.LessonId, tl.Lesson.Name })
                .Distinct()
                .Select(tl => new SelectListItem
                {
                    Value = tl.LessonId.ToString(),
                    Text = tl.Name
                })
                .ToListAsync();

            model.Groups = await context.Groups
                .Where(g => g.TeacherId == teacherId)
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                })
                .ToListAsync();
        }

        private Task<Guid?> ResolveTeacherIdForGroupActionsAsync()
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Task.FromResult<Guid?>(null);
            }

            return Task.FromResult<Guid?>(Guid.Parse(currentUserId));
        }
    }
}
