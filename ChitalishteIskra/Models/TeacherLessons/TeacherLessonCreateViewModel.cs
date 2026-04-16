using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.TeacherLesson
{
    public class TeacherLessonCreateViewModel
    {
        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        public Guid LessonId { get; set; }

        public IEnumerable<SelectListItem> Teachers { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Lessons { get; set; }
            = new List<SelectListItem>();
    }
}
