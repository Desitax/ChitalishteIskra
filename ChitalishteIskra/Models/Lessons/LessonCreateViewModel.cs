using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChitalishteIskra.Models.Lessons
{
    public class LessonCreateViewModel
    {
        public string Name { get; set; } = string.Empty;

        public Guid LessonId { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> Lessons { get; set; }
            = new List<SelectListItem>();
    }
}