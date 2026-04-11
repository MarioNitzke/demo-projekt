namespace PhysioBook.Configurations;

public static class DatabaseConfiguration
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<PhysioBookContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var hasMigrations = context.Database.GetMigrations().Any();

        if (hasMigrations)
        {
            await context.Database.MigrateAsync();
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
        }

        const string adminEmail = "admin@physiobook.local";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var created = await userManager.CreateAsync(adminUser, adminPassword);
            if (!created.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", created.Errors.Select(x => x.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
        }
    }
}

