using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.BookLessons;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<BookLessonIndexDto>> GetAllAsync()
        {
            var teachers = await userManager.GetUsersInRoleAsync("Teacher");
            var teachersIds = teachers.Select(t => t.Id).ToList();

            return await context.BookLessons
                .Include(b => b.Teacher)
                .Include(b => b.Lesson)
                .Where(x => teachersIds.Contains(x.Teacher.Id))
                .Select(b => new BookLessonIndexDto
                {
                    Id = b.Id,
                    Date = b.Date,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    TeacherName = b.Teacher.FirstName + " " + b.Teacher.LastName,
                    LessonName = b.Lesson.Name
                })
                .ToListAsync();
        }

        public async Task<BookLessonCreatePageDto> GetCreatePageDataAsync()
        {
            var lessons = await context.Lessons
                .Where(l => l.TypeName == LessonTypeName.Individual)
                .Select(l => new BookLessonOptionDto
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
                .Select(x => new BookLessonOptionDto
                {
                    Value = x.Id.ToString(),
                    Text = x.Teacher.FirstName + " " + x.Teacher.LastName
                           + " - " + x.Date.ToString()
                           + " - " + x.StartTime.ToString()
                           + " - " + x.EndTime.ToString()
                })
                .ToListAsync();

            return new BookLessonCreatePageDto
            {
                Lessons = lessons,
                AvailableSlots = availableSlots
            };
        }

        public async Task CreateAsync(CreateBookLessonDto model)
        {
            var slot = await context.TeacherAvailabilities
                .FirstOrDefaultAsync(x => x.Id == model.TeacherAvailabilityId && x.IsAvailable);

            if (slot == null)
            {
                throw new ArgumentException("Избраният час вече не е свободен.");
            }

            var lesson = await context.Lessons
                .FirstOrDefaultAsync(l => l.Id == model.LessonId);

            if (lesson == null || lesson.TypeName != LessonTypeName.Individual)
            {
                throw new ArgumentException("Невалиден урок.");
            }

            var booking = new BookLesson
            {
                Id = Guid.NewGuid(),
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                TeacherId = slot.TeacherId,
                LessonId = model.LessonId,
                StudentId = model.StudentId
            };

            slot.IsAvailable = false;

            await context.BookLessons.AddAsync(booking);
            await context.SaveChangesAsync();
        }
    }
}
