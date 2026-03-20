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

        public bool IsApprovedTeacher { get; set; } = false;
        public bool IsTeacherRequest { get; set; } = false;

		public ICollection<BookLesson> BookLessons { get; set; } = new List<BookLesson>();

        public ICollection<TeacherAvailability> TeacherAvailabilities { get; set; } = new List<TeacherAvailability>();

        public ICollection<TeacherEvent> TeacherEvents { get; set; } = new List<TeacherEvent>();

        public ICollection<GroupStudent> GroupStudents { get; set; } = new List<GroupStudent>();
    }    
  
}
