using ChitalishteIskra.Models.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static ChitalishteIskra.Data.Entities.LessonType;

namespace ChitalishteIskra.Models.BookLessons
{
    public class BookLessonCreateViewModel
    {
        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        public Guid LessonId { get; set; }


        [Required]
        public LessonTypeName SelectedLessonType { get; set; }

        public IEnumerable<SelectListItem> Teachers { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Lessons { get; set; } = new List<SelectListItem>();
    }
}
