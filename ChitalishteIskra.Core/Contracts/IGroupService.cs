using ChitalishteIskra.Core.DTOs.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.Contracts
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupDto>> GetAllAsync();

        Task CreateAsync(CreateGroupDto model);

        Task<GroupDto?> GetByIdAsync(Guid id);

        Task UpdateAsync(Guid id, CreateGroupDto model);

        Task DeleteAsync(Guid id);
    }
}
