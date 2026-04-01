using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Models.Groups
{
    public class GroupEditViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public Guid TeacherId { get; set; }
    }
}
