using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Entities
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public ICollection<GroupStudent> GroupStudents { get; set; } = new List<GroupStudent>();
        public ICollection<BookLesson> BookLessons { get; set; } = new List<BookLesson>();
    }
}
