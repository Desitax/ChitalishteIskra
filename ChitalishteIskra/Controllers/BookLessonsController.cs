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

        [Authorize(Roles = "Admin,Student,Teacher")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);

            var data = await bookLessonService.GetAllAsync(
                currentUserId,
                User.IsInRole("Admin"),
                User.IsInRole("Teacher"),
                User.IsInRole("Student"));

            var model = data.Select(b => new BookLessonIndexViewModel
            {
                Id = b.Id,
                Date = b.Date,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                TeacherName = b.TeacherName,
                LessonName = b.LessonName,
                GroupName = b.GroupName,
                AcceptedStudents = b.AcceptedStudents
            });

            return View(model);
        }

        [Authorize(Roles = "Student")]
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

        [Authorize(Roles = "Student")]
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
                if (string.IsNullOrWhiteSpace(model.TeacherAvailabilityId))
                {
                    ModelState.AddModelError(nameof(model.TeacherAvailabilityId), "Избери свободен час.");
                    await PopulateCreateViewModelAsync(model);
                    return View(model);
                }

                var parts = model.TeacherAvailabilityId.Split('|');

                if (parts.Length != 3)
                {
                    ModelState.AddModelError(nameof(model.TeacherAvailabilityId), "Невалиден избор на час.");
                    await PopulateCreateViewModelAsync(model);
                    return View(model);
                }

                var dto = new CreateBookLessonDto
                {
                    TeacherId = model.TeacherId,
                    LessonId = model.LessonId,
                    TeacherAvailabilityId = Guid.Parse(parts[0]),
                    StudentId = currentUserId,
                    Date = model.Date,
                    StartTime = TimeOnly.Parse(parts[1]),
                    EndTime = TimeOnly.Parse(parts[2])
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
                    EndTime = model.EndTime,
                    SelectedStudentIds = model.SelectedStudentIds ?? new List<Guid>()
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

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var booking = await context.BookLessons
               .Include(b => b.Teacher)
               .Include(b => b.Group)
               .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);

            if (User.IsInRole("Teacher") && booking.TeacherId != currentUserId)
            {
                return Forbid();
            }

            var model = new BookLessonEditViewModel
            {
                Id = booking.Id,
                Date = booking.Date,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                TeacherId = booking.TeacherId,
                LessonId = booking.LessonId,
                GroupId = booking.GroupId
            };

            await PopulateEditViewModelAsync(model, booking.TeacherId);
            ViewBag.SelectedTeacherName = booking.Teacher.FirstName + " " + booking.Teacher.LastName;

            return View(model);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            var booking = await context.BookLessons
                .Include(b => b.Teacher)
                .Include(b => b.Group)
                .FirstOrDefaultAsync(b => b.Id == model.Id);

            if (booking == null)
            {
                return NotFound();
            }

            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);
            bool isTeacher = User.IsInRole("Teacher");

            if (isTeacher && booking.TeacherId != currentUserId)
            {
                return Forbid();
            }

            if (isTeacher)
            {
                model.TeacherId = currentUserId;
            }

            bool teacherCanTeachLesson = await context.TeacherLessons.AnyAsync(tl =>
                tl.TeacherId == model.TeacherId &&
                tl.LessonId == model.LessonId);

            if (!teacherCanTeachLesson)
            {
                ModelState.AddModelError(nameof(model.LessonId), "Този учител не преподава избрания урок.");
            }

            if (model.GroupId.HasValue)
            {
                bool validGroup = await context.Groups.AnyAsync(g =>
                    g.Id == model.GroupId.Value &&
                    g.TeacherId == model.TeacherId);

                if (!validGroup)
                {
                    ModelState.AddModelError(nameof(model.GroupId), "Избраната възрастова група не принадлежи на този учител.");
                }
            }

            bool hasConflict = await context.BookLessons.AnyAsync(b =>
                b.Id != model.Id &&
                b.TeacherId == model.TeacherId &&
                b.Date == model.Date &&
                model.StartTime < b.EndTime &&
                model.EndTime > b.StartTime);

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "Учителят вече има запазен час в този диапазон.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateEditViewModelAsync(model, model.TeacherId);
                ViewBag.SelectedTeacherName = booking.Teacher.FirstName + " " + booking.Teacher.LastName;
                return View(model);
            }

            booking.Date = model.Date;
            booking.StartTime = model.StartTime;
            booking.EndTime = model.EndTime;
            booking.TeacherId = model.TeacherId;
            booking.LessonId = model.LessonId;
            booking.GroupId = model.GroupId;

            await context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Часът беше редактиран успешно.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var booking = await context.BookLessons.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string? currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserIdString))
            {
                return Unauthorized();
            }

            Guid currentUserId = Guid.Parse(currentUserIdString);

            if (User.IsInRole("Teacher") && booking.TeacherId != currentUserId)
            {
                return Forbid();
            }

            var groupResponses = await context.GroupLessonResponses
                .Where(r => r.BookLessonId == booking.Id)
                .ToListAsync();

            if (groupResponses.Any())
            {
                context.GroupLessonResponses.RemoveRange(groupResponses);
            }

            context.BookLessons.Remove(booking);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Часът беше изтрит успешно.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> GetTeacherBookingData(Guid teacherId, DateOnly date)
        {
            var data = await bookLessonService.GetTeacherBookingDataAsync(teacherId, date);

            return Json(new
            {
                lessons = data.Lessons.Select(x => new { value = x.Value, text = x.Text }),
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

                model.WorkingHours = teacherData.WorkingHours;
            }
        }

        private async Task PopulateCreateGroupViewModelAsync(BookLessonCreateGroupViewModel model, Guid teacherId)
        {
            model.Lessons = await context.TeacherLessons
                .Where(tl => tl.TeacherId == teacherId
                    && tl.TypeName == LessonTypeName.Group
                    && !tl.Lesson.IsDeleted)
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

            var students = await userManager.GetUsersInRoleAsync("Student");

            model.Students = students
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.FirstName} {s.LastName} ({s.UserName})"
                })
                .ToList();
        }

        private async Task PopulateEditViewModelAsync(BookLessonEditViewModel model, Guid selectedTeacherId)
        {
            bool isTeacher = User.IsInRole("Teacher");

            if (isTeacher)
            {
                var teacher = await context.Users.FirstOrDefaultAsync(u => u.Id == selectedTeacherId);

                model.Teachers = teacher == null
                    ? new List<SelectListItem>()
                    : new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Value = teacher.Id.ToString(),
                            Text = teacher.FirstName + " " + teacher.LastName
                        }
                    };
            }
            else
            {
                var teacherIds = await userManager.GetUsersInRoleAsync("Teacher");

                model.Teachers = teacherIds
                    .OrderBy(t => t.FirstName)
                    .ThenBy(t => t.LastName)
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.FirstName + " " + t.LastName
                    })
                    .ToList();
            }

            model.Lessons = await context.TeacherLessons
                .Where(tl => tl.TeacherId == selectedTeacherId && !tl.Lesson.IsDeleted)
                .Select(tl => new SelectListItem
                {
                    Value = tl.LessonId.ToString(),
                    Text = tl.Lesson.Name
                })
                .Distinct()
                .ToListAsync();

            model.Groups = await context.Groups
                .Where(g => g.TeacherId == selectedTeacherId)
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