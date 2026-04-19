namespace ChitalishteIskra.Core.DTOs.Lessons
{
    public class TeacherCreateLessonDto
    {
        public Guid TeacherId { get; set; }

        public Guid LessonId { get; set; }

        public string TypeName { get; set; } = string.Empty;
    }
}