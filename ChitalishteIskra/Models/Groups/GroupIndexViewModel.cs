namespace ChitalishteIskra.Models.Groups
{
    public class GroupIndexViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Guid TeacherId { get; set; }

        public string TeacherName { get; set; } = null!;
    }
}
