using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonType = ChitalishteIskra.Data.Entities.LessonType;


namespace ChitalishteIskra.Data

{
    public class ChitalishteIskraDbContext: IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public ChitalishteIskraDbContext(DbContextOptions<ChitalishteIskraDbContext> options)
		: base(options)
		{
		}

		
        public DbSet<LessonType> Types { get; set; } = null!;
        public DbSet<TeacherEvent> TeacherEvents { get; set; } = null!;
        public DbSet<StudentBookLesson> StudentBookLessons { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<BookLesson> BookLessons { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			
			builder.Entity<BookLesson>()
				.HasOne(bl => bl.Teacher)
				.WithMany(u => u.TeacherBookLessons)
				.HasForeignKey(bl => bl.TeacherId)
				.OnDelete(DeleteBehavior.Restrict); 

			
			builder.Entity<StudentBookLesson>()
				.HasOne(sbl => sbl.Student)
				.WithMany(u => u.StudentBookLessons)
				.HasForeignKey(sbl => sbl.StudentId)
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

			
			builder.Entity<Lesson>()
				.HasOne(l => l.Type)
				.WithMany(t => t.Lessons)
				.HasForeignKey(l => l.TypeId)
				.OnDelete(DeleteBehavior.Restrict);

		
			builder.Entity<BookLesson>()
				.HasOne(bl => bl.Lesson)
				.WithMany(l => l.BookLessons)
				.HasForeignKey(bl => bl.LessonId)
				.OnDelete(DeleteBehavior.Cascade);

			
			builder.Entity<StudentBookLesson>()
				.HasOne(sbl => sbl.BookLesson)
				.WithMany(bl => bl.StudentBookLessons)
				.HasForeignKey(sbl => sbl.BookLessonId)
				.OnDelete(DeleteBehavior.Cascade);

			
			builder.Entity<StudentBookLesson>()
				.HasIndex(x => new { x.StudentId, x.BookLessonId })
				.IsUnique();

			
			builder.Entity<TeacherEvent>()
				.HasIndex(x => new { x.TeacherId, x.EventId })
				.IsUnique();

		
			builder.Entity<LessonType>()
				.HasIndex(x => x.Name)
				.IsUnique();
		}

	}
}
