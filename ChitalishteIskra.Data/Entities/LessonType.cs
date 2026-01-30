using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Entities
{
    public class LessonType
    {
        [Key]
        public Guid Id { get; set; }
		public LessonTypeName Name { get; set; }
		public enum LessonTypeName { Individual, Group }
		public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
	}
}
