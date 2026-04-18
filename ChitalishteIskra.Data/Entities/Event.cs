using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Data.Entities
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public string Location { get; set; } = null!;

        [Required]
        public string Description { get; set; } 

        public string? ImageUrl { get; set; }

        public ICollection<TeacherEvent> TeacherEvents { get; set; } = new List<TeacherEvent>();
    }
}