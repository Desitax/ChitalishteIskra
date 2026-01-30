using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Entities
{
    public class StudentBookLesson
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        [ForeignKey(nameof(BookLesson))]
        public Guid BookLessonId { get; set; }
        public BookLesson BookLesson { get; set; } = null!;
    }
}
