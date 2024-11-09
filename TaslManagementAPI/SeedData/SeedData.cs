using Microsoft.AspNetCore.Identity;
using TaslManagementAPI.Models;
using TaslManagementAPI.Utilities;

namespace TaslManagementAPI.SeedData
{
    public class SeedData
    {
        public async static Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = { RolesConstants.Admin, RolesConstants.User };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@test.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdminUser = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "Administrator"
                };

                var result = await userManager.CreateAsync(newAdminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, RolesConstants.Admin);
                }
            }
        }
    }
}
