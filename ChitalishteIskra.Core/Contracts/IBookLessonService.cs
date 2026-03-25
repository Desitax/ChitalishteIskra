using ChitalishteIskra.Core.DTOs.BookLessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.Contracts
{
    public interface IBookLessonService
    {
        Task<IEnumerable<BookLessonIndexDto>> GetAllAsync();

        Task<BookLessonCreatePageDto> GetCreatePageDataAsync();

        Task CreateAsync(CreateBookLessonDto model);
    }
}
