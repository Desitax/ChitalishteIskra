using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Configurations
{
    public class TeacherLessonConfiguration: IEntityTypeConfiguration<TeacherLesson>
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
        }
    }
}
