using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Events
{
    public class EventEditViewModel
    {
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
    }
}
