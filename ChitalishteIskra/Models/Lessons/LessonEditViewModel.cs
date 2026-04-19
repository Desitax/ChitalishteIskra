using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Lessons
{
    public class LessonEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Въведи име на предмет")]
        public string Name { get; set; } = string.Empty;
    }
}