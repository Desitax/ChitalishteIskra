using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.TeacherAvailabilities;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Core.Services
{
    public class TeacherAvailabilityService : ITeacherAvailabilityService
    {
        private readonly ChitalishteIskraDbContext context;

        public TeacherAvailabilityService(ChitalishteIskraDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<TeacherAvailabilityDto>> GetAllAsync()
        {
            return await context.TeacherAvailabilities
                .Include(t => t.Teacher)
                .Select(t => new TeacherAvailabilityDto
                {
                    Id = t.Id,
                    DayOfWeek = t.DayOfWeek,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    IsAvailable = t.IsAvailable,
                    TeacherName = t.Teacher.FirstName + " " + t.Teacher.LastName
                })
                .ToListAsync();
        }

        public async Task CreateAsync(CreateTeacherAvailabilityDto model)
        {
            var availability = new TeacherAvailability
            {
                Id = Guid.NewGuid(),
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TeacherId = model.TeacherId,
                IsAvailable = true
            };

            await context.TeacherAvailabilities.AddAsync(availability);
            await context.SaveChangesAsync();
        }

        public async Task<TeacherAvailabilityDto> GetByIdAsync(Guid id)
        {
            var entity = await context.TeacherAvailabilities
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ArgumentException("Not found");
            }

            return new TeacherAvailabilityDto
            {
                Id = entity.Id,
                DayOfWeek = entity.DayOfWeek,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                IsAvailable = entity.IsAvailable,
                TeacherName = entity.Teacher.FirstName + " " + entity.Teacher.LastName
            };
        }

        public async Task UpdateAsync(Guid id, CreateTeacherAvailabilityDto model)
        {
            var entity = await context.TeacherAvailabilities.FindAsync(id);

            if (entity == null)
            {
                throw new ArgumentException("Not found");
            }

            entity.DayOfWeek = model.DayOfWeek;
            entity.StartTime = model.StartTime;
            entity.EndTime = model.EndTime;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var availability = await context.TeacherAvailabilities.FindAsync(id);

            if (availability == null)
            {
                throw new ArgumentException("Availability not found");
            }

            context.TeacherAvailabilities.Remove(availability);
            await context.SaveChangesAsync();
        }
    }
}
