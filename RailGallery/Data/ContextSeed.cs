using Microsoft.AspNetCore.Identity;
using RailGallery.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Data
{
    /// <summary>
    /// Context Seed class allows to seed the database with user roles and two default user accounts.
    /// 
    /// Moderator User: 
    ///     - Username: Moderator
    ///     - Email: moderator@railgallery.org
    ///     - Password: Password1!
    /// 
    /// Basic User:
    ///     - Username: User
    ///     - Email: user@railgallery.org
    ///     - Password: Password1!
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class ContextSeed
    {
        /// <summary>
        /// Seed user idenitiy roles: Moderator and Basic User.
        /// </summary>
        /// <param name="roleManager">Reference to Role Manager object.</param>
        /// <returns></returns>
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.BasicUser.ToString()));
        }

        /// <summary>
        /// Seed Moderator and Basic User users.
        /// </summary>
        /// <param name="userManager">Reference to the user manager object.</param>
        /// <returns></returns>
        public static async Task SeedUsersAsyc(UserManager<ApplicationUser> userManager)
        {

            // Define the moderator user
            var moderatorUser = new ApplicationUser
            {
                UserName = "Moderator",
                Email = "moderator@railgallery.org",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            // If the user doesn't exist in the database
            if (userManager.Users.All(u => u.Id != moderatorUser.Id))
            {
                // Make sure the email is not used by another user
                var user = await userManager.FindByEmailAsync(moderatorUser.Email);
                // If the user with this email is not found, save the user in the database
                if (user == null)
                {
                    // Save the Moderator user to the database and assign the moderator and basic user roles to the user
                    await userManager.CreateAsync(moderatorUser, "Password1!");
                    await userManager.AddToRoleAsync(moderatorUser, Enums.Roles.BasicUser.ToString());
                    await userManager.AddToRoleAsync(moderatorUser, Enums.Roles.Moderator.ToString());
                }

            }

            // Define the Basic User user
            var basicUser = new ApplicationUser
            {
                UserName = "User",
                Email = "user@railgallery.org",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            // If the user doesn't exist in the database
            if (userManager.Users.All(u => u.Id != basicUser.Id))
            {
                // Make sure the email is not used by another user
                var user = await userManager.FindByEmailAsync(basicUser.Email);
                // If the user with this email is not found, save the user in the database
                if (user == null)
                {
                    // Save the Basic User user to the database and assign the moderator and basic user role to the user
                    await userManager.CreateAsync(basicUser, "Password1!");
                    await userManager.AddToRoleAsync(basicUser, Enums.Roles.BasicUser.ToString());
                }

            }
        }
    }
}
