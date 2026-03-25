using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Lessons
{
    public class LessonCreateViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string TypeName { get; set; } = null!;
    }
}
