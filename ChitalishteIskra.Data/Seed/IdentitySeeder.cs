using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Data.Seed
{
    public class IdentitySeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roles = { "Admin", "Teacher", "Parent", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }

        public static async Task SeedAdminAsync(UserManager<User> userManager)
        {

            string username = "admin";
            string email = "admin@admin.com";
            var adminUser = await userManager.FindByNameAsync(username);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = username,
                    Email = email,
                    FirstName = "Admin",
                    LastName = "Adminov",
                    Age = 30
                };

                await userManager.CreateAsync(adminUser, "admin");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        public static async Task SeedTeacherAsync(UserManager<User> userManager)
        {
            string username = "teacher";
            string email = "teacher@teacher.com";

            var teacherUser = await userManager.FindByNameAsync(username);

            if (teacherUser == null)
            {
                teacherUser = new User
                {
                    UserName = username,
                    Email = email,
                    FirstName = "Ivan",
                    LastName = "Ivanov",
                    Age = 35
                };

                await userManager.CreateAsync(teacherUser, "teacher");
            }

            if (!await userManager.IsInRoleAsync(teacherUser, "Teacher"))
            {
                await userManager.AddToRoleAsync(teacherUser, "Teacher");
            }
        }

        public static async Task SeedStudentAsync(UserManager<User> userManager)
        {
            string username = "student";
            string email = "student@student.com";

            var studentUser = await userManager.FindByNameAsync(username);

            if (studentUser == null)
            {
                studentUser = new User
                {
                    UserName = username,
                    Email = email,
                    FirstName = "Petko",
                    LastName = "Petkov",
                    Age = 18
                };

                await userManager.CreateAsync(studentUser, "student");
            }

            if (!await userManager.IsInRoleAsync(studentUser, "Student"))
            {
                await userManager.AddToRoleAsync(studentUser, "Student");
            }
        }

        public static async Task SeedParentAsync(UserManager<User> userManager)
        {
            string username = "parent";
            string email = "parent@parent.com";

            var parentUser = await userManager.FindByNameAsync(username);

            if (parentUser == null)
            {
                parentUser = new User
                {
                    UserName = username,
                    Email = email,
                    FirstName = "Maria",
                    LastName = "Petrova",
                    Age = 40
                };

                await userManager.CreateAsync(parentUser, "parent");
            }

            if (!await userManager.IsInRoleAsync(parentUser, "Parent"))
            {
                await userManager.AddToRoleAsync(parentUser, "Parent");
            }
        }

    }
}
