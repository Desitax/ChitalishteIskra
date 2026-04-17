using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Lessons
{
    public class LessonCreateViewModel
    {
        [Required(ErrorMessage = "Въведи име на предмет")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Избери тип")]
        public string TypeName { get; set; } = string.Empty;
    }
}
