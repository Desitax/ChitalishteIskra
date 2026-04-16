using ChitalishteIskra.Core.DTOs.TeacherLessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.Contracts
{
    public interface ITeacherLessonService
    {
        Task<IEnumerable<TeacherLessonIndexDto>> GetAllAsync();
        Task<TeacherLessonCreatePageDto> GetCreatePageDataAsync();
        Task CreateAsync(CreateTeacherLessonDto model);
    }
}
