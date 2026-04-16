using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Entities
{
    public class TeacherLesson
    {
        [Key] 
        public Guid Id { get; set; }
        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;
    }
}
