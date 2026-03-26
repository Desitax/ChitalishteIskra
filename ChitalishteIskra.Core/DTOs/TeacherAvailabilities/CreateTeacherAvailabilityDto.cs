using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.DTOs.TeacherAvailabilities
{
    public class CreateTeacherAvailabilityDto
    {
        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public Guid TeacherId { get; set; }
    }
}
