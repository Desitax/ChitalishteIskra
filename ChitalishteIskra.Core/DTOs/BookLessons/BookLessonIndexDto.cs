using System;

namespace ChitalishteIskra.Core.DTOs.BookLessons
{
    public class BookLessonIndexDto
    {
        public Guid Id { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string TeacherName { get; set; } = null!;

        public string LessonName { get; set; } = null!;

        public string GroupName { get; set; } = "-";

        public string AcceptedStudents { get; set; } = "-";
    }
}