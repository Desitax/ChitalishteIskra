using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.DTOs.BookLessons
{
    public class CreateGroupLessonDto
    {
        public Guid TeacherId { get; set; }
        public Guid LessonId { get; set; }
        public Guid GroupId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
