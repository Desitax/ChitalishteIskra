using System;
using System.ComponentModel.DataAnnotations;

namespace ChitalishteIskra.Data.Entities
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = null!;

        public DateTime SentOn { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}