using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.TeacherLessons;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Core.Services
{
    public class TeacherLessonService : ITeacherLessonService
    {
        private readonly ChitalishteIskraDbContext context;
        private readonly UserManager<User> userManager;

        public TeacherLessonService(
            ChitalishteIskraDbContext context,
            UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<TeacherLessonIndexDto>> GetAllAsync()
        {
            return await context.TeacherLessons
                .Include(x => x.Teacher)
                .Include(x => x.Lesson)
                .Select(x => new TeacherLessonIndexDto
                {
                    Id = x.Id,
                    TeacherName = x.Teacher.FirstName + " " + x.Teacher.LastName,
                    LessonName = x.Lesson.Name,
                    LessonType = x.Lesson.TypeName.ToString()
                })
                .ToListAsync();
        }

        public async Task<TeacherLessonCreatePageDto> GetCreatePageDataAsync()
        {
            var teachers = await userManager.GetUsersInRoleAsync("Teacher");
            var lessons = await context.Lessons
                .Where(l => !l.IsDeleted)
                .ToListAsync();

            return new TeacherLessonCreatePageDto
            {
                Teachers = teachers
                    .Where(t => t.IsApprovedTeacher)
                    .Select(t => new TeacherLessonOptionDto
                    {
                        Value = t.Id.ToString(),
                        Text = t.FirstName + " " + t.LastName
                    })
                    .ToList(),

                Lessons = lessons
                    .Select(l => new TeacherLessonOptionDto
                    {
                        Value = l.Id.ToString(),
                        Text = l.Name
                    })
                    .ToList()
            };
        }

        public async Task CreateAsync(CreateTeacherLessonDto model)
        {
            bool alreadyExists = await context.TeacherLessons
                .AnyAsync(x => x.TeacherId == model.TeacherId && x.LessonId == model.LessonId);

            if (alreadyExists)
            {
                throw new ArgumentException("Този предмет вече е назначен на учителя.");
            }

            var entity = new TeacherLesson
            {
                Id = Guid.NewGuid(),
                TeacherId = model.TeacherId,
                LessonId = model.LessonId
            };

            await context.TeacherLessons.AddAsync(entity);
            await context.SaveChangesAsync();
        }
    }
}