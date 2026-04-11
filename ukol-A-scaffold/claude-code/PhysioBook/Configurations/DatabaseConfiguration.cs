using Microsoft.AspNetCore.Identity;
using PhysioBook.Auth;
using PhysioBook.Data.Entities;

namespace PhysioBook.Configurations;

public static class DatabaseConfiguration
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<Data.PhysioBookContext>();
            await context.Database.EnsureCreatedAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await SeedRolesAsync(roleManager);

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            await SeedAdminUserAsync(userManager);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while initializing the database");
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@physiobook.cz";
        const string adminPassword = "Admin123!";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin != null) return;

        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "PhysioBook",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
        }
    }
}
