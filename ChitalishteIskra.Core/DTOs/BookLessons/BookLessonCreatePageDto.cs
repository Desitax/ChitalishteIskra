using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.DTOs.BookLessons
{
    public class BookLessonCreatePageDto
    {
        public IEnumerable<BookLessonOptionDto> Teachers { get; set; }
            = new List<BookLessonOptionDto>();
    }
}
