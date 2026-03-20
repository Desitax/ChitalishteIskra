using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Entities
{
    public class GroupStudent
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid GroupId { get; set; }
        public Group Group { get; set; } = null!;

        [Required]
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;
    }
}
