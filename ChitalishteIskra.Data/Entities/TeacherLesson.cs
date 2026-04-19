using System.ComponentModel.DataAnnotations;

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

        public Lesson.LessonTypeName TypeName { get; set; }
    }
}