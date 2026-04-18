using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Events;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Core.Services
{
    public class EventService : IEventService
    {
        private readonly ChitalishteIskraDbContext context;

        public EventService(ChitalishteIskraDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync()
        {
            return await context.Events
                .OrderBy(e => e.Date)
                .ThenBy(e => e.StartTime)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Date = e.Date,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Location = e.Location,
                    Description = e.Description,
                    ImageUrl = e.ImageUrl
                })
                .ToListAsync();
        }

        public async Task CreateAsync(CreateEventDto model)
        {
            var entity = new Event
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Location = model.Location,
                Description = model.Description,
                ImageUrl = model.ImageUrl
            };

            await context.Events.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task<EventDto?> GetByIdAsync(Guid id)
        {
            return await context.Events
                .Where(e => e.Id == id)
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Date = e.Date,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Location = e.Location,
                    Description = e.Description,
                    ImageUrl = e.ImageUrl
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Guid id, CreateEventDto model)
        {
            var entity = await context.Events.FindAsync(id);

            if (entity == null)
            {
                throw new ArgumentException("Event not found");
            }

            entity.Name = model.Name;
            entity.Date = model.Date;
            entity.StartTime = model.StartTime;
            entity.EndTime = model.EndTime;
            entity.Location = model.Location;
            entity.Description = model.Description;
            entity.ImageUrl = model.ImageUrl;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await context.Events.FindAsync(id);

            if (entity == null)
            {
                throw new ArgumentException("Event not found");
            }

            context.Events.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}