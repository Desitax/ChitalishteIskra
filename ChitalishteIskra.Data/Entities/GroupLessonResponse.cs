using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChitalishteIskra.Data.Entities
{
    public class GroupLessonResponse
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid BookLessonId { get; set; }
        public BookLesson BookLesson { get; set; } = null!;

        [Required]
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        [Required]
        public GroupLessonResponseStatus Status { get; set; } = GroupLessonResponseStatus.Pending;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public enum GroupLessonResponseStatus
        {
            Pending = 0,
            Accepted = 1,
            Declined = 2
        }
    }
}
