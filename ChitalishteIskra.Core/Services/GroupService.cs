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
                 .Include(g => g.Teacher)
                 .Select(g => new GroupDto
                 {
                     Id = g.Id,
                     Name = g.Name,
                     TeacherId = g.TeacherId,
                     TeacherName = g.Teacher.FirstName + " " + g.Teacher.LastName
                 })
                 .ToListAsync();
        }

        public async Task CreateAsync(CreateGroupDto model)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                TeacherId = model.TeacherId
            };

            await context.Groups.AddAsync(group);
            await context.SaveChangesAsync();
        }

        public async Task<GroupDto?> GetByIdAsync(Guid id)
        {
            return await context.Groups
                .Include(g => g.Teacher)
                .Where(g => g.Id == id)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    TeacherId = g.TeacherId,
                    TeacherName = g.Teacher.FirstName + " " + g.Teacher.LastName
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
            group.TeacherId = model.TeacherId;

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
