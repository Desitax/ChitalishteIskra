using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.BookLessons
{
    public class BookLessonCreateViewModel
    {
        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public Guid LessonId { get; set; }

        [Required(ErrorMessage = "Избери свободен час.")]
        public string TeacherAvailabilityId { get; set; } = string.Empty;

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public IEnumerable<SelectListItem> Teachers { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Lessons { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AvailableSlots { get; set; } = new List<SelectListItem>();
        public IEnumerable<string> TeacherGroups { get; set; } = new List<string>();
        public IEnumerable<string> WorkingHours { get; set; } = new List<string>();
    }
}