namespace ChitalishteIskra.Core.DTOs.TeacherAvailabilities
{
    public class TeacherAvailabilityDto
    {
        public Guid Id { get; set; }

        public Guid TeacherId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsAvailable { get; set; }

        public string TeacherName { get; set; } = null!;
    }
}