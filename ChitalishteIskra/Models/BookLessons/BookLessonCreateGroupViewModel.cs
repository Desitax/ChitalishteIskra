using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.BookLessons
{
    public class BookLessonCreateGroupViewModel
    {
        [Required]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public Guid LessonId { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        public List<Guid> SelectedStudentIds { get; set; } = new List<Guid>();

        public IEnumerable<SelectListItem> Lessons { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Groups { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Students { get; set; } = new List<SelectListItem>();
    }
}