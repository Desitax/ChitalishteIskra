using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Entities
{
    public class BookLesson
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [ForeignKey(nameof(Teacher))]
        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

		[ForeignKey(nameof(Lesson))]
		public Guid LessonId { get; set; }
		public Lesson Lesson { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public Guid? StudentId { get; set; }
        public User? Student { get; set; }

        public Guid? GroupId { get; set; }
        public Group? Group { get; set; }

	}
}
