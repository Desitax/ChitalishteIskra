using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.User
{
    public class UserRegisterViewModel
    {

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public int Age { get; set; }
    }
}
