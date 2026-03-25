using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.User
{
    public class StudentRegisterViewModel
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;

        public StudentInfoRegisterViewModel StudentInfo { get; set; } = new();

        public ParentInfoRegisterViewModel ParentInfo { get; set; } = new();
    }
}
