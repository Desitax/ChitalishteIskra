using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Groups
{
    public class GroupCreateViewModel
    {
        [Required(ErrorMessage = "Моля въведи име на групата.")]
        public string Name { get; set; } = null!;

        [Required]
        public Guid TeacherId { get; set; }
    }
}