using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Data.Entities
{
    public class Lesson
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public ICollection<BookLesson> BookLessons { get; set; } = new List<BookLesson>();
        public ICollection<TeacherLesson> TeacherLessons { get; set; } = new List<TeacherLesson>();

        public bool IsDeleted { get; set; }

        public enum LessonTypeName
        {
            Individual,
            Group
        }
    }
}