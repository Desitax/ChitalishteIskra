using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.User
{
    public class StudentInfoRegisterViewModel
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        [Range(1, 120)]
        public int Age { get; set; }
    }
}
