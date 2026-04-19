using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Lessons;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Core.Services
{
    public class LessonService : ILessonService
    {
        private readonly ChitalishteIskraDbContext context;

        public LessonService(ChitalishteIskraDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<LessonDto>> GetAllAsync()
        {
            return await context.Lessons
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.Name)
                .Select(l => new LessonDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    TypeName = string.Empty
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<LessonDto>> GetByTeacherIdAsync(Guid teacherId)
        {
            return await context.TeacherLessons
                .Where(tl => tl.TeacherId == teacherId && !tl.Lesson.IsDeleted)
                .OrderBy(tl => tl.Lesson.Name)
                .Select(tl => new LessonDto
                {
                    Id = tl.LessonId,
                    Name = tl.Lesson.Name,
                    TypeName = tl.TypeName.ToString()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<LessonDto>> GetAssignedToTeacherAsync(Guid teacherId)
        {
            return await context.TeacherLessons
                .Where(tl => tl.TeacherId == teacherId && !tl.Lesson.IsDeleted)
                .OrderBy(tl => tl.Lesson.Name)
                .Select(tl => new LessonDto
                {
                    Id = tl.LessonId,
                    Name = tl.Lesson.Name,
                    TypeName = tl.TypeName.ToString()
                })
                .ToListAsync();
        }

        public async Task CreateAsync(CreateLessonDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Името на предмета е задължително.");
            }

            bool alreadyExists = await context.Lessons
                .AnyAsync(l =>
                    !l.IsDeleted &&
                    l.Name.ToLower() == model.Name.Trim().ToLower());

            if (alreadyExists)
            {
                throw new ArgumentException("Такъв предмет вече съществува.");
            }

            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                Name = model.Name.Trim(),
                IsDeleted = false
            };

            await context.Lessons.AddAsync(lesson);
            await context.SaveChangesAsync();
        }

        public async Task CreateForTeacherAsync(TeacherCreateLessonDto model)
        {
            var teacherLesson = await context.TeacherLessons
                .Include(tl => tl.Lesson)
                .FirstOrDefaultAsync(tl =>
                    tl.TeacherId == model.TeacherId &&
                    tl.LessonId == model.LessonId &&
                    !tl.Lesson.IsDeleted);

            if (teacherLesson == null)
            {
                throw new ArgumentException("Нямате право да избирате този предмет.");
            }

            if (string.IsNullOrWhiteSpace(model.TypeName))
            {
                throw new ArgumentException("Избери тип.");
            }

            var parsedType = Enum.Parse<Lesson.LessonTypeName>(model.TypeName);
            teacherLesson.TypeName = parsedType;

            await context.SaveChangesAsync();
        }

        public async Task<LessonDto?> GetByIdAsync(Guid id)
        {
            return await context.Lessons
                .Where(l => l.Id == id && !l.IsDeleted)
                .Select(l => new LessonDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    TypeName = string.Empty
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Guid id, CreateLessonDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Името на предмета е задължително.");
            }

            var lesson = await context.Lessons
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);

            if (lesson == null)
            {
                throw new ArgumentException("Предметът не е намерен.");
            }

            bool alreadyExists = await context.Lessons
                .AnyAsync(l =>
                    l.Id != id &&
                    !l.IsDeleted &&
                    l.Name.ToLower() == model.Name.Trim().ToLower());

            if (alreadyExists)
            {
                throw new ArgumentException("Вече съществува друг предмет със същото име.");
            }

            lesson.Name = model.Name.Trim();

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var lesson = await context.Lessons
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);

            if (lesson == null)
            {
                throw new ArgumentException("Предметът не е намерен.");
            }

            lesson.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }
}