using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.User
{
    public class ParentInfoRegisterViewModel
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Range(1, 120)]
        public int? Age { get; set; }
    }
}
