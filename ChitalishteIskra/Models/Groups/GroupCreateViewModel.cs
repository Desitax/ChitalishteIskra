using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Groups
{
    public class GroupCreateViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public Guid TeacherId { get; set; }
    }
}
