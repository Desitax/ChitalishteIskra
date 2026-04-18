using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Events
{
    public class EventEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Датата е задължителна.")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Началният час е задължителен.")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "Крайният час е задължителен.")]
        public TimeOnly EndTime { get; set; }

        [Required(ErrorMessage = "Местоположението е задължително.")]
        [StringLength(200)]
        public string Location { get; set; } = null!;

        [Required(ErrorMessage = "Описанието е задължително.")]
        [StringLength(2000)]
        public string Description { get; set; } = null!;

        public string? ExistingImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}