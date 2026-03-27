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
    public class BookLessonConfiguration: IEntityTypeConfiguration<BookLesson>
    {
        public void Configure(EntityTypeBuilder<BookLesson> builder)
        {
            builder
                .HasOne(bl => bl.Teacher)
                .WithMany(u => u.BookLessons)
                .HasForeignKey(bl => bl.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(bl => bl.Lesson)
                .WithMany(l => l.BookLessons)
                .HasForeignKey(bl => bl.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(bl => bl.Student)
                .WithMany()
                .HasForeignKey(bl => bl.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(bl => bl.Group)
                .WithMany(g => g.BookLessons)
                .HasForeignKey(bl => bl.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasIndex(bl => new { bl.TeacherId, bl.Date, bl.StartTime, bl.EndTime })
                .IsUnique();
        }
    }
}
