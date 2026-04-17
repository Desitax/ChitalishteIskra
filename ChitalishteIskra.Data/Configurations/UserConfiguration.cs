using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChitalishteIskra.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();

            var hasher = new PasswordHasher<User>();

            // ADMIN
            var admin = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "Adminov",
                Age = 30,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            admin.PasswordHash = hasher.HashPassword(admin, "admin");

            // TEACHER
            var teacher = new User
            {
                Id = Guid.Parse("35a5aa59-3911-4fdd-83ca-38f0d7bb91b7"),
                UserName = "teacher",
                NormalizedUserName = "TEACHER",
                Email = "teacher@teacher.com",
                NormalizedEmail = "TEACHER@TEACHER.COM",
                EmailConfirmed = true,
                FirstName = "Ivan",
                LastName = "Ivanov",
                Age = 35,
                IsApprovedTeacher = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            teacher.PasswordHash = hasher.HashPassword(teacher, "teacher");

            // STUDENT
            var student = new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                UserName = "student",
                NormalizedUserName = "STUDENT",
                Email = "student@student.com",
                NormalizedEmail = "STUDENT@STUDENT.COM",
                EmailConfirmed = true,
                FirstName = "Petko",
                LastName = "Petkov",
                Age = 18,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            student.PasswordHash = hasher.HashPassword(student, "student");

            // PARENT
            var parent = new User
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                UserName = "parent",
                NormalizedUserName = "PARENT",
                Email = "parent@parent.com",
                NormalizedEmail = "PARENT@PARENT.COM",
                EmailConfirmed = true,
                FirstName = "Maria",
                LastName = "Petrova",
                Age = 40,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            parent.PasswordHash = hasher.HashPassword(parent, "parent");

            builder.HasData(admin, teacher, student, parent);
        }
    }
}