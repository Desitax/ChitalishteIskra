using ChitalishteIskra.Core.DTOs.Lessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.Contracts
{
    public interface ILessonService
    {
        Task<IEnumerable<LessonDto>> GetAllAsync();

        Task CreateAsync(CreateLessonDto model);

        Task<LessonDto?> GetByIdAsync(Guid id);

        Task UpdateAsync(Guid id, CreateLessonDto model);

        Task DeleteAsync(Guid id);
    }
}
