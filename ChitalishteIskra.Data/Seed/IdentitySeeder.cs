using ChitalishteIskra.Data.Entities;
using Microsoft.AspNetCore.Identity;

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
                    await roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                }
            }
        }

        public static async Task AssignUsersToRolesAsync(UserManager<User> userManager)
        {
            await AssignRole(userManager, "admin", "Admin");
            await AssignRole(userManager, "teacher", "Teacher");
            await AssignRole(userManager, "student", "Student");
            await AssignRole(userManager, "parent", "Parent");
        }

        private static async Task AssignRole(
            UserManager<User> userManager,
            string username,
            string roleName)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user != null && !await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}