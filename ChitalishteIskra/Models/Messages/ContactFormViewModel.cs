using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Messages
{
    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Полето за име е задължително.")]
        [StringLength(100, ErrorMessage = "Името може да бъде до 100 символа.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Полето за email е задължително.")]
        [EmailAddress(ErrorMessage = "Моля, въведете валиден email адрес.")]
        [StringLength(150, ErrorMessage = "Email адресът може да бъде до 150 символа.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Полето за съобщение е задължително.")]
        [StringLength(2000, ErrorMessage = "Съобщението може да бъде до 2000 символа.")]
        public string Content { get; set; } = null!;
    }
}