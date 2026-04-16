using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.DTOs.BookLessons
{
    public class BookLessonTeacherInfoDto
    {
        public IEnumerable<BookLessonOptionDto> Lessons { get; set; }
            = new List<BookLessonOptionDto>();

        public IEnumerable<BookLessonOptionDto> Groups { get; set; }
            = new List<BookLessonOptionDto>();

        public IEnumerable<string> WorkingHours { get; set; }
            = new List<string>();

        public IEnumerable<BookLessonOptionDto> AvailableSlots { get; set; }
            = new List<BookLessonOptionDto>();
    }
}
