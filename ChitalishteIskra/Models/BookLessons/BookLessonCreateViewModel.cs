using ChitalishteIskra.Models.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.BookLessons
{
    public class BookLessonCreateViewModel
    {
        [Required]
        public Guid LessonId { get; set; }

        [Required]
        public Guid TeacherAvailabilityId { get; set; }

        public IEnumerable<SelectListItem> Lessons { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> AvailableSlots { get; set; } = new List<SelectListItem>();
    }
}
