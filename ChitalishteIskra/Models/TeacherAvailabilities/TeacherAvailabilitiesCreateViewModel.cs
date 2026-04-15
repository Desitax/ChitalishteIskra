using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.TeacherAvailabilities
{
    public class TeacherAvailabilitiesCreateViewModel
    {
        [Required]
        public Guid TeacherId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;

    }
}
