using FirstProject.Data;
using FirstProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Utils;

public static class AdminDefaultCreate
{
    public static async Task Initializer(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<ApplicationDbContext>();

        if (context is null)
        {
            Console.WriteLine("Error occured during context providing");
            return;
        }
        var admin = new ApplicationUser
        {
            FirstName = "ADMIN",
            LastName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            UserName = "admin@example.com",
            NormalizedUserName = "Admin",
            PhoneNumber = "+111111111111",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        if (!await context.Users.AnyAsync(u => u.UserName == admin.UserName))
        {
            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(admin,"AdminPassword");
            admin.PasswordHash = hashed;

            var adminStore = new UserStore<ApplicationUser>(context);
            var result = await adminStore.CreateAsync(admin);
        }

        await context.SaveChangesAsync();

    }

    public static async Task<IdentityResult> AdminRoleAssign(IServiceProvider serviceProvider)
    {
        UserManager<ApplicationUser> _userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
        if (_userManager is null)
        {
            return null;
        }

        ApplicationUser adminInstance = await _userManager.FindByEmailAsync("admin@example.com");
        var result = await _userManager.AddToRoleAsync(adminInstance, "Admin");
        
        return result;
    }
}

public static class RolesDefaultCreate
{
    public static async Task Initializer(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<ApplicationDbContext>();
        if (context is null)
        {
            Console.WriteLine("Error occured during context providing");
            return;
        }

        List<string> roles = new List<string>() {"Admin", "User", "Student"};
        foreach (var roleName in roles)
        {
            var role = new RoleStore<IdentityRole>(context);
            if (!await context.Roles.AnyAsync(i => i.Name == roleName))
            {
                await role.CreateAsync(new IdentityRole(roleName){NormalizedName = roleName.ToUpper()});
            }
        }

        await AdminDefaultCreate.AdminRoleAssign(serviceProvider);

        await context.SaveChangesAsync();

    }
}
