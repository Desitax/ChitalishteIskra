using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChitalishteIskra.Data.Configurations
{
    public class TeacherLessonConfiguration : IEntityTypeConfiguration<TeacherLesson>
    {
        public void Configure(EntityTypeBuilder<TeacherLesson> builder)
        {
            builder.HasKey(tl => tl.Id);

            builder
                .HasOne(tl => tl.Teacher)
                .WithMany(u => u.TeacherLessons)
                .HasForeignKey(tl => tl.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(tl => tl.Lesson)
                .WithMany(l => l.TeacherLessons)
                .HasForeignKey(tl => tl.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasIndex(tl => new { tl.TeacherId, tl.LessonId })
                .IsUnique();

            builder.HasData(
                new TeacherLesson
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    TeacherId = Guid.Parse("35a5aa59-3911-4fdd-83ca-38f0d7bb91b7"),
                    LessonId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
                },
                new TeacherLesson
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    TeacherId = Guid.Parse("35a5aa59-3911-4fdd-83ca-38f0d7bb91b7"),
                    LessonId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
                }
            );
        }
    }
}
