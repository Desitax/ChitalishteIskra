using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.DTOs.Groups;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChitalishteIskra.Core.Services
{
    public class GroupService : IGroupService
    {
        private readonly ChitalishteIskraDbContext context;

        public GroupService(ChitalishteIskraDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync(Guid currentUserId, bool isAdmin, bool isTeacher)
        {
            var query = context.Groups
                .Include(g => g.Teacher)
                .AsQueryable();

            if (isAdmin)
            {
                // admin вижда всички групи
            }
            else if (isTeacher)
            {
                query = query.Where(g => g.TeacherId == currentUserId);
            }
            else
            {
                query = query.Where(g => false);
            }

            return await query
                .OrderBy(g => g.Name)
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
            bool teacherExists = await context.Users.AnyAsync(u => u.Id == model.TeacherId);

            if (!teacherExists)
            {
                throw new ArgumentException("Невалиден учител.");
            }

            bool duplicateNameForTeacher = await context.Groups.AnyAsync(g =>
                g.TeacherId == model.TeacherId &&
                g.Name.ToLower() == model.Name.ToLower());

            if (duplicateNameForTeacher)
            {
                throw new ArgumentException("Вече имаш група с това име.");
            }

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

        public async Task UpdateAsync(Guid id, CreateGroupDto model, Guid currentUserId, bool isAdmin)
        {
            var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                throw new ArgumentException("Групата не е намерена.");
            }

            if (!isAdmin && group.TeacherId != currentUserId)
            {
                throw new UnauthorizedAccessException("Нямаш право да редактираш тази група.");
            }

            if (!isAdmin)
            {
                model.TeacherId = currentUserId;
            }

            bool duplicateNameForTeacher = await context.Groups.AnyAsync(g =>
                g.Id != id &&
                g.TeacherId == model.TeacherId &&
                g.Name.ToLower() == model.Name.ToLower());

            if (duplicateNameForTeacher)
            {
                throw new ArgumentException("Вече имаш група с това име.");
            }

            group.Name = model.Name;
            group.TeacherId = model.TeacherId;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, Guid currentUserId, bool isAdmin)
        {
            var group = await context.Groups
                .Include(g => g.GroupStudents)
                .Include(g => g.BookLessons)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                throw new ArgumentException("Групата не е намерена.");
            }

            if (!isAdmin && group.TeacherId != currentUserId)
            {
                throw new UnauthorizedAccessException("Нямаш право да изтриеш тази група.");
            }

            if (group.BookLessons.Any())
            {
                throw new ArgumentException("Не може да изтриеш група, към която вече има създадени групови уроци.");
            }

            if (group.GroupStudents.Any())
            {
                context.GroupStudents.RemoveRange(group.GroupStudents);
            }

            context.Groups.Remove(group);
            await context.SaveChangesAsync();
        }
    }
}