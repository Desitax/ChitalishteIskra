using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Lessons;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;
using static ChitalishteIskra.Data.Entities.Lesson;

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
                    TypeName = l.TypeName.ToString()
                })
                .ToListAsync();
        }

        public async Task CreateAsync(CreateLessonDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Името на предмета е задължително.");
            }

            var parsedType = Enum.Parse<LessonTypeName>(model.TypeName);

            bool alreadyExists = await context.Lessons
                .AnyAsync(l =>
                    !l.IsDeleted &&
                    l.Name.ToLower() == model.Name.Trim().ToLower() &&
                    l.TypeName == parsedType);

            if (alreadyExists)
            {
                throw new ArgumentException("Такъв предмет вече съществува.");
            }

            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                Name = model.Name.Trim(),
                TypeName = parsedType,
                IsDeleted = false
            };

            await context.Lessons.AddAsync(lesson);
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
                    TypeName = l.TypeName.ToString()
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

            var parsedType = Enum.Parse<LessonTypeName>(model.TypeName);

            bool alreadyExists = await context.Lessons
                .AnyAsync(l =>
                    l.Id != id &&
                    !l.IsDeleted &&
                    l.Name.ToLower() == model.Name.Trim().ToLower() &&
                    l.TypeName == parsedType);

            if (alreadyExists)
            {
                throw new ArgumentException("Вече съществува друг предмет със същото име и тип.");
            }

            lesson.Name = model.Name.Trim();
            lesson.TypeName = parsedType;

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