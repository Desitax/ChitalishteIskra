using ChitalishteIskra.Core.DTOs.TeacherAvailabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.Contracts
{
    public interface ITeacherAvailabilityService
    {
        Task<IEnumerable<TeacherAvailabilityDto>> GetAllAsync();

        Task CreateAsync(CreateTeacherAvailabilityDto model);

        Task<TeacherAvailabilityDto> GetByIdAsync(Guid id);

        Task UpdateAsync(Guid id, CreateTeacherAvailabilityDto model);

        Task DeleteAsync(Guid id);
    }
}
