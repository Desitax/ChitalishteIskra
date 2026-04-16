using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.TeacherAvailabilities
{
    public class TeacherAvailabilityIndexViewModel
    {
        public Guid Id { get; set; }
        public string TeacherName { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
