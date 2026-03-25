using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.DTOs.BookLessons
{
    public class BookLessonIndexDto
    {
        public Guid Id { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string TeacherName { get; set; } = null!;

        public string LessonName { get; set; } = null!;
    }
}
