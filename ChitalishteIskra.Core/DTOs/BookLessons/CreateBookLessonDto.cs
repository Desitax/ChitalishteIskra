namespace ChitalishteIskra.Core.DTOs.BookLessons
{
    public class CreateBookLessonDto
    {
        public Guid TeacherId { get; set; }
        public Guid LessonId { get; set; }
        public Guid TeacherAvailabilityId { get; set; }
        public Guid StudentId { get; set; }
        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}