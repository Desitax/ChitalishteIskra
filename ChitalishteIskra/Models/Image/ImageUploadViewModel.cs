using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Image
{
    public class ImageUploadViewModel
    {
        [Required(ErrorMessage = "Моля, избери снимка.")]
        public IFormFile? Picture { get; set; }

        public string? ImageUrl { get; set; }
    }
}
