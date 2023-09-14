using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ZeusApp.Application.Enums;
using ZeusApp.Infrastructure.Identity.Models;

namespace ZeusApp.Infrastructure.Identity.Seeds;

public static class DefaultBasicUser
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        //var newUser = new ApplicationUser
        //{
        //    UserName = "firmaadmin@example.com",
        //    Email = "firmaadmin@example.com",
        //    FirstName = "Firma",
        //    LastName = "Admin",
        //    EmailConfirmed = true,
        //    PhoneNumber = "5558536677",
        //    PhoneNumberConfirmed = true,
        //    IsActive = true,
        //    KullaniciTuru = KullaniciTurleri.DenetimFirmasi
        //};

        //var user = await userManager.FindByEmailAsync(newUser.Email);
        //if (user == null)
        //{
        //    await userManager.CreateAsync(newUser, "102030");
        //    await userManager.AddToRoleAsync(newUser, Roles.FirmaAdmin.ToString());
        //}
    }
}
