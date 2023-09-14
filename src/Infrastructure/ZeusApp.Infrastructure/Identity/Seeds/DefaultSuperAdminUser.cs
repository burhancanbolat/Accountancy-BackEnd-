using Microsoft.AspNetCore.Identity;
using ZeusApp.Application.Enums;
using ZeusApp.Infrastructure.Identity.Models;

namespace ZeusApp.Infrastructure.Identity.Seeds;

public static class DefaultSuperAdminUser
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var newUser = new ApplicationUser
        {
            UserName = "superadmin@example.com",
            Email = "superadmin@example.com",
            FirstName = "Super",
            LastName = "Admin",
            Title = "BT Yazılım",
            EmailConfirmed = true,
            PhoneNumber = "5458536677",
            PhoneNumberConfirmed = true,
            IsActive = true,
            UserType = UserType.ZeusApp
        };

        var user = await userManager.FindByEmailAsync(newUser.Email);
        if (user == null)
        {
            await userManager.CreateAsync(newUser, "102030");
            await userManager.AddToRoleAsync(newUser, Role.SuperAdmin.ToString());
        }
    }
}
