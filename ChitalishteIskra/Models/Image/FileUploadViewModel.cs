using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Image
{
    public class FileUploadViewModel
    {
        [Required(ErrorMessage = "Моля, избери файл.")]
        public IFormFile? Document { get; set; }

        public string? FileUrl { get; set; }

        public string? FileName { get; set; }
    }
}
