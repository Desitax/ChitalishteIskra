namespace ChitalishteIskra.Core.DTOs.Events
{
    public class CreateEventDto
    {
        public string Name { get; set; } = null!;

        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string Location { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }
}