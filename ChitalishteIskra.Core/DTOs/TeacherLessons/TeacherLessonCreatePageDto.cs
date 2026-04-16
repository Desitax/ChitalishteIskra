using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.DTOs.TeacherLessons
{
    public class TeacherLessonCreatePageDto
    {
        public IEnumerable<TeacherLessonOptionDto> Teachers { get; set; }
            = new List<TeacherLessonOptionDto>();

        public IEnumerable<TeacherLessonOptionDto> Lessons { get; set; }
            = new List<TeacherLessonOptionDto>();
    }
}
