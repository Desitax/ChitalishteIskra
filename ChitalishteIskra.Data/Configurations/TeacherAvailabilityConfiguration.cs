using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .HasIndex(ta => new { ta.TeacherId, ta.Date, ta.StartTime, ta.EndTime })
                .IsUnique();
        }
    }
}
