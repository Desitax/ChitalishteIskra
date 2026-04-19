namespace ChitalishteIskra.Core.DTOs.TeacherLessons
{
    public class TeacherLessonIndexDto
    {
        public Guid Id { get; set; }

        public string TeacherName { get; set; } = string.Empty;

        public string LessonName { get; set; } = string.Empty;

        public string LessonType { get; set; } = string.Empty;
    }
}