using ChitalishteIskra.Core.DTOs.Groups;

namespace ChitalishteIskra.Core.Contracts
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupDto>> GetAllAsync(Guid currentUserId, bool isAdmin, bool isTeacher);

        Task CreateAsync(CreateGroupDto model);

        Task<GroupDto?> GetByIdAsync(Guid id);

        Task UpdateAsync(Guid id, CreateGroupDto model, Guid currentUserId, bool isAdmin);

        Task DeleteAsync(Guid id, Guid currentUserId, bool isAdmin);
    }
}