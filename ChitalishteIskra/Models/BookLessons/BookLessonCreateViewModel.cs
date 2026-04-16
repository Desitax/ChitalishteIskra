using ChitalishteIskra.Models.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.BookLessons
{
    public class BookLessonCreateViewModel
    {
        [Required(ErrorMessage = "Избери учител")]
        public Guid TeacherId { get; set; }

        [Required(ErrorMessage = "Избери дата")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Избери предмет")]
        public Guid LessonId { get; set; }

        [Required(ErrorMessage = "Избери свободен час")]
        public Guid TeacherAvailabilityId { get; set; }

        public IEnumerable<SelectListItem> Teachers { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Lessons { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> AvailableSlots { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<string> TeacherGroups { get; set; }
            = new List<string>();

        public IEnumerable<string> WorkingHours { get; set; }
            = new List<string>();
    }
}
