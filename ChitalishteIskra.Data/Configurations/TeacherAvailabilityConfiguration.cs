using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChitalishteIskra.Data.Configurations
{
    public class TeacherAvailabilityConfiguration : IEntityTypeConfiguration<TeacherAvailability>
    {
        public void Configure(EntityTypeBuilder<TeacherAvailability> builder)
        {
            builder
                .HasOne(ta => ta.Teacher)
                .WithMany(u => u.TeacherAvailabilities)
                .HasForeignKey(ta => ta.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasIndex(ta => new { ta.TeacherId, ta.DayOfWeek, ta.StartTime, ta.EndTime })
                .IsUnique();

            builder.HasData(
                new TeacherAvailability
                {
                    Id = Guid.Parse("61154f0c-8726-4dee-96a9-0c7c37916a41"),
                    TeacherId = Guid.Parse("35a5aa59-3911-4fdd-83ca-38f0d7bb91b7"),
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(12, 0),
                    IsAvailable = true
                },
                new TeacherAvailability
                {
                    Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                    TeacherId = Guid.Parse("35a5aa59-3911-4fdd-83ca-38f0d7bb91b7"),
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeOnly(13, 0),
                    EndTime = new TimeOnly(17, 0),
                    IsAvailable = true
                }
            );
        }
    }
}