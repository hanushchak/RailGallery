using Microsoft.AspNetCore.Identity;
using RailGallery.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Data
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.BasicUser.ToString()));
        }

        public static async Task SeedUsersAsyc(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Super Admin User
/*            var adminUser = new ApplicationUser
            {
                UserName = "Admin",
                Email = "admin@railgallery.org",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != adminUser.Id))
            {
                var user = await userManager.FindByEmailAsync(adminUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(adminUser, "Password1!");
                    await userManager.AddToRoleAsync(adminUser, Enums.Roles.BasicUser.ToString());
                    await userManager.AddToRoleAsync(adminUser, Enums.Roles.Moderator.ToString());
                }

            }*/

            // Seed Moderator User
            var moderatorUser = new ApplicationUser
            {
                UserName = "Moderator",
                Email = "moderator@railgallery.org",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != moderatorUser.Id))
            {
                var user = await userManager.FindByEmailAsync(moderatorUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(moderatorUser, "Password1!");
                    await userManager.AddToRoleAsync(moderatorUser, Enums.Roles.BasicUser.ToString());
                    await userManager.AddToRoleAsync(moderatorUser, Enums.Roles.Moderator.ToString());
                }

            }

            // Seed Basic User
            var basicUser = new ApplicationUser
            {
                UserName = "User",
                Email = "user@railgallery.org",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != basicUser.Id))
            {
                var user = await userManager.FindByEmailAsync(basicUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(basicUser, "Password1!");
                    await userManager.AddToRoleAsync(basicUser, Enums.Roles.BasicUser.ToString());
                }

            }
        }
    }
}
