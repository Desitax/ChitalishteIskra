namespace ChitalishteIskra.Core.DTOs.BookLessons
{
    public class GroupLessonInvitationDto
    {
        public Guid BookLessonId { get; set; }

        public string LessonName { get; set; } = string.Empty;

        public string GroupName { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
