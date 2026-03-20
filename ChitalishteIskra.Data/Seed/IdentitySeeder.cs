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
                if(!await roleManager.RoleExistsAsync(role))
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

    }
}
