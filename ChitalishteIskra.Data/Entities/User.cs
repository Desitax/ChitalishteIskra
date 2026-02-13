    using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Entities
{
    public class User:IdentityUser<Guid>
	{
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public int Age { get; set; }

		public ICollection<StudentBookLesson> StudentBookLessons { get; set; } = new List<StudentBookLesson>();
		public ICollection<BookLesson> TeacherBookLessons { get; set; } = new List<BookLesson>();

		public ICollection<TeacherEvent> TeacherEvents { get; set; } = new List<TeacherEvent>();
	}    
  
}
