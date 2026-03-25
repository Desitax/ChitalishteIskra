using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.DTOs.Lessons
{
    public class LessonDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string TypeName { get; set; } = null!;
    }
}
