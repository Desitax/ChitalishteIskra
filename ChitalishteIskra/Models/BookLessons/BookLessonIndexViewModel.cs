using ChitalishteIskra.Models.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

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

        //public int StudentsCount { get; set; }

    }
}
