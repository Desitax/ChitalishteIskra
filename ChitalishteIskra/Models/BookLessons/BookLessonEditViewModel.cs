using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.BookLessons
{
    public class BookLessonEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Моля изберете дата")]
        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Моля въведете начален час")]
        [DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "Моля въведете краен час")]
        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        [Required(ErrorMessage = "Изберете учител")]
        public Guid TeacherId { get; set; }

        [Required(ErrorMessage = "Изберете урок")]
        public Guid LessonId { get; set; }

        public Guid? GroupId { get; set; }

        public IEnumerable<SelectListItem> Teachers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Lessons { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Groups { get; set; } = new List<SelectListItem>();
    }
}