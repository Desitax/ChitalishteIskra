using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.BookLessons;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static ChitalishteIskra.Data.Entities.GroupLessonResponse;
using static ChitalishteIskra.Data.Entities.Lesson;

namespace ChitalishteIskra.Core.Services
{
    public class BookLessonService : IBookLessonService
    {
        private readonly ChitalishteIskraDbContext context;
        private readonly UserManager<User> userManager;

        public BookLessonService(
            ChitalishteIskraDbContext context,
            UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<BookLessonIndexDto>> GetAllAsync(Guid currentUserId, bool isAdmin, bool isTeacher, bool isStudent)
        {
            var query = context.BookLessons
                .Include(b => b.Teacher)
                .Include(b => b.Lesson)
                .Include(b => b.Student)
                .Include(b => b.Group)
                .AsQueryable();

            if (isAdmin)
            {
            }
            else if (isTeacher)
            {
                query = query.Where(b => b.TeacherId == currentUserId);
            }
            else if (isStudent)
            {
                query = query.Where(b =>
                    b.StudentId == currentUserId ||
                    context.GroupLessonResponses.Any(r => r.BookLessonId == b.Id && r.StudentId == currentUserId));
            }
            else
            {
                query = query.Where(b => false);
            }

            var bookings = await query
                .OrderBy(b => b.Date)
                .ThenBy(b => b.StartTime)
                .ToListAsync();

            var bookingIds = bookings.Select(b => b.Id).ToList();

            var acceptedResponses = await context.GroupLessonResponses
                .Where(r => bookingIds.Contains(r.BookLessonId) && r.Status == GroupLessonResponseStatus.Accepted)
                .Join(context.Users,
                    r => r.StudentId,
                    u => u.Id,
                    (r, u) => new
                    {
                        r.BookLessonId,
                        StudentName = u.FirstName + " " + u.LastName
                    })
                .ToListAsync();

            var acceptedStudentsByLesson = acceptedResponses
                .GroupBy(x => x.BookLessonId)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(", ", g.Select(x => x.StudentName).Distinct().OrderBy(x => x)));

            var result = bookings.Select(b => new BookLessonIndexDto
            {
                Id = b.Id,
                Date = b.Date,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                TeacherName = b.Teacher.FirstName + " " + b.Teacher.LastName,
                LessonName = b.Lesson.Name,
                GroupName = b.Group != null ? b.Group.Name : "-",
                AcceptedStudents = b.Student != null
                    ? b.Student.FirstName + " " + b.Student.LastName
                    : acceptedStudentsByLesson.ContainsKey(b.Id)
                        ? acceptedStudentsByLesson[b.Id]
                        : "-"
            });

            return result.ToList();
        }

        public async Task<BookLessonCreatePageDto> GetCreatePageDataAsync()
        {
            var teachers = await userManager.GetUsersInRoleAsync("Teacher");

            return new BookLessonCreatePageDto
            {
                Teachers = teachers
                    .Where(t => t.IsApprovedTeacher)
                    .Select(t => new BookLessonOptionDto
                    {
                        Value = t.Id.ToString(),
                        Text = t.FirstName + " " + t.LastName
                    })
                    .ToList()
            };
        }

        public async Task<BookLessonTeacherInfoDto> GetTeacherBookingDataAsync(Guid teacherId, DateOnly date)
        {
            var dayOfWeek = date.ToDateTime(TimeOnly.MinValue).DayOfWeek;

            var lessons = await context.TeacherLessons
                .Where(tl => tl.TeacherId == teacherId
                          && tl.Lesson.TypeName == LessonTypeName.Individual
                          && !tl.Lesson.IsDeleted)
                .Select(tl => new { tl.LessonId, tl.Lesson.Name })
                .Distinct()
                .Select(tl => new BookLessonOptionDto
                {
                    Value = tl.LessonId.ToString(),
                    Text = tl.Name
                })
                .ToListAsync();

            var groups = await context.Groups
                .Where(g => g.TeacherId == teacherId)
                .Select(g => new BookLessonOptionDto
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                })
                .ToListAsync();

            var workingHoursEntities = await context.TeacherAvailabilities
                .Where(ta => ta.TeacherId == teacherId
                             && ta.DayOfWeek == dayOfWeek
                             && ta.IsAvailable)
                .OrderBy(ta => ta.StartTime)
                .ToListAsync();

            var bookedSlots = await context.BookLessons
                .Where(bl => bl.TeacherId == teacherId && bl.Date == date)
                .Select(bl => new { bl.StartTime, bl.EndTime })
                .ToListAsync();

            var availableSlots = new List<BookLessonOptionDto>();

            foreach (var wh in workingHoursEntities)
            {
                var currentStart = wh.StartTime;
                var rangeEnd = wh.EndTime;

                while (currentStart.AddHours(1) <= rangeEnd)
                {
                    var currentEnd = currentStart.AddHours(1);

                    bool isBooked = bookedSlots.Any(bs =>
                        bs.StartTime == currentStart && bs.EndTime == currentEnd);

                    if (!isBooked)
                    {
                        availableSlots.Add(new BookLessonOptionDto
                        {
                            Value = $"{wh.Id}|{currentStart:HH\\:mm}|{currentEnd:HH\\:mm}",
                            Text = $"{currentStart:HH\\:mm} - {currentEnd:HH\\:mm}"
                        });
                    }

                    currentStart = currentEnd;
                }
            }

            var workingHours = workingHoursEntities
                .Select(wh => $"{wh.DayOfWeek} : {wh.StartTime:HH\\:mm} - {wh.EndTime:HH\\:mm}")
                .ToList();

            return new BookLessonTeacherInfoDto
            {
                Lessons = lessons,
                Groups = groups,
                WorkingHours = workingHours,
                AvailableSlots = availableSlots
            };
        }

        public async Task CreateAsync(CreateBookLessonDto model)
        {
            var lesson = await context.Lessons
                .FirstOrDefaultAsync(l => l.Id == model.LessonId && !l.IsDeleted);

            if (lesson == null || lesson.TypeName != LessonTypeName.Individual)
            {
                throw new ArgumentException("Избраният урок не е индивидуален.");
            }

            bool teacherCanTeachLesson = await context.TeacherLessons
                .AnyAsync(tl => tl.TeacherId == model.TeacherId && tl.LessonId == model.LessonId);

            if (!teacherCanTeachLesson)
            {
                throw new ArgumentException("Този учител не преподава избрания предмет.");
            }

            var slotRange = await context.TeacherAvailabilities
                .FirstOrDefaultAsync(x =>
                    x.Id == model.TeacherAvailabilityId &&
                    x.TeacherId == model.TeacherId &&
                    x.IsAvailable);

            if (slotRange == null)
            {
                throw new ArgumentException("Избраният час вече не е свободен.");
            }

            var requestedDay = model.Date.ToDateTime(TimeOnly.MinValue).DayOfWeek;
            if (slotRange.DayOfWeek != requestedDay)
            {
                throw new ArgumentException("Избраният час не съответства на избраната дата.");
            }

            if (model.EndTime <= model.StartTime)
            {
                throw new ArgumentException("Крайният час трябва да е след началния.");
            }

            if (model.EndTime != model.StartTime.AddHours(1))
            {
                throw new ArgumentException("Индивидуалният урок трябва да е точно 1 час.");
            }

            if (model.StartTime < slotRange.StartTime || model.EndTime > slotRange.EndTime)
            {
                throw new ArgumentException("Избраният час е извън работното време на учителя.");
            }

            bool alreadyBooked = await context.BookLessons.AnyAsync(bl =>
                bl.TeacherId == model.TeacherId &&
                bl.Date == model.Date &&
                bl.StartTime == model.StartTime &&
                bl.EndTime == model.EndTime);

            if (alreadyBooked)
            {
                throw new ArgumentException("Този час вече е зает.");
            }

            var booking = new BookLesson
            {
                Id = Guid.NewGuid(),
                TeacherId = model.TeacherId,
                LessonId = model.LessonId,
                StudentId = model.StudentId,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                GroupId = null
            };

            await context.BookLessons.AddAsync(booking);
            await context.SaveChangesAsync();
        }

        public async Task CreateGroupAsync(CreateGroupLessonDto model)
        {
            var lesson = await context.Lessons
                .FirstOrDefaultAsync(l => l.Id == model.LessonId && !l.IsDeleted);

            if (lesson == null || lesson.TypeName != LessonTypeName.Group)
            {
                throw new ArgumentException("Избраният урок не е групов.");
            }

            bool teacherCanTeachLesson = await context.TeacherLessons
                .AnyAsync(tl => tl.TeacherId == model.TeacherId && tl.LessonId == model.LessonId);

            if (!teacherCanTeachLesson)
            {
                throw new ArgumentException("Този учител не преподава избрания групов предмет.");
            }

            var group = await context.Groups
                .FirstOrDefaultAsync(g => g.Id == model.GroupId && g.TeacherId == model.TeacherId);

            if (group == null)
            {
                throw new ArgumentException("Избраната група не принадлежи на този учител.");
            }

            var dayOfWeek = model.Date.ToDateTime(TimeOnly.MinValue).DayOfWeek;

            bool teacherIsWorking = await context.TeacherAvailabilities.AnyAsync(ta =>
                ta.TeacherId == model.TeacherId &&
                ta.DayOfWeek == dayOfWeek &&
                ta.IsAvailable &&
                ta.StartTime <= model.StartTime &&
                ta.EndTime >= model.EndTime);

            if (!teacherIsWorking)
            {
                throw new ArgumentException("Урокът е извън работното време на учителя.");
            }

            bool hasConflict = await context.BookLessons.AnyAsync(bl =>
                bl.TeacherId == model.TeacherId &&
                bl.Date == model.Date &&
                model.StartTime < bl.EndTime &&
                model.EndTime > bl.StartTime);

            if (hasConflict)
            {
                throw new ArgumentException("Учителят вече има урок в този часови диапазон.");
            }

            var booking = new BookLesson
            {
                Id = Guid.NewGuid(),
                TeacherId = model.TeacherId,
                LessonId = model.LessonId,
                GroupId = model.GroupId,
                StudentId = null,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            await context.BookLessons.AddAsync(booking);

            var selectedStudentIds = model.SelectedStudentIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (selectedStudentIds.Any())
            {
                var validStudents = await context.Users
                    .Where(u => selectedStudentIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync();

                foreach (var studentId in validStudents)
                {
                    var response = new GroupLessonResponse
                    {
                        Id = Guid.NewGuid(),
                        BookLessonId = booking.Id,
                        StudentId = studentId,
                        Status = GroupLessonResponseStatus.Pending
                    };

                    await context.GroupLessonResponses.AddAsync(response);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GroupLessonInvitationDto>> GetStudentGroupLessonsAsync(Guid studentId)
        {
            return await context.GroupLessonResponses
                .Where(x => x.StudentId == studentId)
                .Include(x => x.BookLesson)
                    .ThenInclude(bl => bl.Lesson)
                .Include(x => x.BookLesson)
                    .ThenInclude(bl => bl.Teacher)
                .Include(x => x.BookLesson)
                    .ThenInclude(bl => bl.Group)
                .OrderBy(x => x.BookLesson.Date)
                .ThenBy(x => x.BookLesson.StartTime)
                .Select(x => new GroupLessonInvitationDto
                {
                    BookLessonId = x.BookLessonId,
                    LessonName = x.BookLesson.Lesson.Name,
                    GroupName = x.BookLesson.Group != null ? x.BookLesson.Group.Name : string.Empty,
                    TeacherName = x.BookLesson.Teacher.FirstName + " " + x.BookLesson.Teacher.LastName,
                    Date = x.BookLesson.Date,
                    StartTime = x.BookLesson.StartTime,
                    EndTime = x.BookLesson.EndTime,
                    Status = x.Status == GroupLessonResponseStatus.Pending
                        ? "Чака отговор"
                        : x.Status == GroupLessonResponseStatus.Accepted
                            ? "Приет"
                            : "Отказан"
                })
                .ToListAsync();
        }

        public async Task RespondToGroupLessonAsync(Guid bookLessonId, Guid studentId, bool isAccepted)
        {
            var response = await context.GroupLessonResponses
                .Include(x => x.BookLesson)
                .FirstOrDefaultAsync(x => x.BookLessonId == bookLessonId && x.StudentId == studentId);

            if (response == null)
            {
                throw new ArgumentException("Няма намерена покана за този групов урок.");
            }

            if (response.BookLesson.Date < DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentException("Не може да отговаряш на минал урок.");
            }

            response.Status = isAccepted
                ? GroupLessonResponseStatus.Accepted
                : GroupLessonResponseStatus.Declined;

            await context.SaveChangesAsync();
        }
    }
}