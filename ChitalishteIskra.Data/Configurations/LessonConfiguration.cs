using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChitalishteIskra.Data.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder
                .Property(l => l.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasData(
                new Lesson
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Name = "Урок по танци",
                    IsDeleted = false
                },
                new Lesson
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Name = "Групов урок по танци",
                    IsDeleted = false
                }
            );
        }
    }
}