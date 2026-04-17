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

        public List<Guid> SelectedStudentIds { get; set; } = new List<Guid>();
    }
}