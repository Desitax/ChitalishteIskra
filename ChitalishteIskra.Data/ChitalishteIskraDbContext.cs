using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data

{
    public class ChitalishteIskraDbContext: IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public ChitalishteIskraDbContext(DbContextOptions<ChitalishteIskraDbContext> options)
		: base(options)
		{
		}


        public DbSet<TeacherAvailability> TeacherAvailabilities { get; set; } = null!;
        public DbSet<TeacherEvent> TeacherEvents { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<GroupStudent> GroupStudents { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<BookLesson> BookLessons { get; set; } = null!;
        public DbSet<TeacherLesson> TeacherLessons { get; set; } = null!;
        public DbSet<GroupLessonResponse> GroupLessonResponses { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BookLesson>()
                .HasOne(bl => bl.Teacher)
                .WithMany(u => u.BookLessons)
                .HasForeignKey(bl => bl.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BookLesson>()
                .HasOne(bl => bl.Lesson)
                .WithMany(l => l.BookLessons)
                .HasForeignKey(bl => bl.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BookLesson>()
                .HasOne(bl => bl.Student)
                .WithMany()
                .HasForeignKey(bl => bl.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BookLesson>()
                .HasOne(bl => bl.Group)
                .WithMany(g => g.BookLessons)
                .HasForeignKey(bl => bl.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TeacherAvailability>()
                .HasOne(ta => ta.Teacher)
                .WithMany(u => u.TeacherAvailabilities)
                .HasForeignKey(ta => ta.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TeacherEvent>()
                .HasOne(te => te.Teacher)
                .WithMany(u => u.TeacherEvents)
                .HasForeignKey(te => te.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TeacherEvent>()
                .HasOne(te => te.Event)
                .WithMany(e => e.TeacherEvents)
                .HasForeignKey(te => te.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GroupStudent>()
                .HasOne(gs => gs.Group)
                .WithMany(g => g.GroupStudents)
                .HasForeignKey(gs => gs.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GroupStudent>()
                .HasOne(gs => gs.Student)
                .WithMany(u => u.GroupStudents)
                .HasForeignKey(gs => gs.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TeacherEvent>()
                .HasIndex(x => new { x.TeacherId, x.EventId })
                .IsUnique();

            builder.Entity<GroupStudent>()
                .HasIndex(x => new { x.GroupId, x.StudentId })
                .IsUnique();

            builder.Entity<TeacherAvailability>()
                .HasIndex(x => new { x.TeacherId, x.DayOfWeek, x.StartTime, x.EndTime })
                .IsUnique();

            builder.Entity<BookLesson>()
                .HasIndex(x => new { x.TeacherId, x.Date, x.StartTime, x.EndTime })
                .IsUnique();

            builder.Entity<Group>()
                .Property(g => g.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Lesson>()
                .Property(l => l.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Event>()
                .Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Event>()
                .Property(e => e.Location)
                .HasMaxLength(200)
                .IsRequired();

            builder.Entity<User>()
                .Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<User>()
                .Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();
        }

    }
}
