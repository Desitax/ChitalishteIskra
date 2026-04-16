using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.TeacherAvailabilities
{
    public class TeacherAvailabilitiesCreateViewModel
    {
        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}