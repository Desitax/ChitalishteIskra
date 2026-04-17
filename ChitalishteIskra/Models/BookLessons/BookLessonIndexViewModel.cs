using System;

namespace ChitalishteIskra.Models.BookLessons
{
    public class BookLessonIndexViewModel
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