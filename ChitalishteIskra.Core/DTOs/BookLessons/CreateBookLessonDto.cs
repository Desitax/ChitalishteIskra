using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.DTOs.BookLessons
{
    public class CreateBookLessonDto
    {
        public Guid LessonId { get; set; }

        public Guid TeacherAvailabilityId { get; set; }

        public Guid StudentId { get; set; }
    }
}
