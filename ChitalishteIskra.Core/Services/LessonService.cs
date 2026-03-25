using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Lessons;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChitalishteIskra.Data.Entities.Lesson;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Core.Services
{
    public class LessonService:ILessonService
    {
        private readonly ChitalishteIskraDbContext context;

        public LessonService(ChitalishteIskraDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<LessonDto>> GetAllAsync()
        {
            return await context.Lessons
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
            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                TypeName = Enum.Parse<LessonTypeName>(model.TypeName)
            };

            await context.Lessons.AddAsync(lesson);
            await context.SaveChangesAsync();
        }

        public async Task<LessonDto?> GetByIdAsync(Guid id)
        {
            return await context.Lessons
                .Where(l => l.Id == id)
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
            var lesson = await context.Lessons.FindAsync(id);

            if (lesson == null)
            {
                throw new ArgumentException("Lesson not found");
            }

            lesson.Name = model.Name;
            lesson.TypeName = Enum.Parse<LessonTypeName>(model.TypeName);

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var lesson = await context.Lessons.FindAsync(id);

            if (lesson == null)
            {
                throw new ArgumentException("Lesson not found");
            }

            context.Lessons.Remove(lesson);
            await context.SaveChangesAsync();
        }
    }
}
