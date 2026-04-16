using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.DTOs.TeacherLessons
{
    public class CreateTeacherLessonDto
    {
        public Guid TeacherId { get; set; }

        public Guid LessonId { get; set; }
    }
}
