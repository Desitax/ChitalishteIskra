namespace ChitalishteIskra.Core.DTOs.Lessons
{
    public class LessonDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string TypeName { get; set; } = string.Empty;
    }
}