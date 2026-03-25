using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Groups;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.Services
{
    public class GroupService:IGroupService
    {
        private readonly ChitalishteIskraDbContext context;

        public GroupService(ChitalishteIskraDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync()
        {
            return await context.Groups
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();
        }

        public async Task CreateAsync(CreateGroupDto model)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = model.Name
            };

            await context.Groups.AddAsync(group);
            await context.SaveChangesAsync();
        }

        public async Task<GroupDto?> GetByIdAsync(Guid id)
        {
            return await context.Groups
                .Where(g => g.Id == id)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Guid id, CreateGroupDto model)
        {
            var group = await context.Groups.FindAsync(id);

            if (group == null)
            {
                throw new ArgumentException("Group not found");
            }

            group.Name = model.Name;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var group = await context.Groups.FindAsync(id);

            if (group == null)
            {
                throw new ArgumentException("Group not found");
            }

            context.Groups.Remove(group);
            await context.SaveChangesAsync();
        }
    }
}
