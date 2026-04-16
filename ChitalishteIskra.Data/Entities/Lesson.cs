using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Entities
{
    public class Lesson
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        public LessonTypeName TypeName { get; set; }
        public ICollection<BookLesson> BookLessons { get; set; }=new List<BookLesson>();
        public ICollection<TeacherLesson> TeacherLessons { get; set; } = new List<TeacherLesson>();
        public enum LessonTypeName
        {
            Individual,
            Group
        }
    }
}
