namespace PhysioBook.Api.Configurations;

public static class DatabaseConfiguration
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PhysioBookContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
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

            const string adminEmail = "admin@physiobook.demo";
            const string adminPassword = "Admin123!";

            var adminUser = await userManager.Users.FirstOrDefaultAsync(x => x.Email == adminEmail);
            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = adminEmail,
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpperInvariant(),
                    NormalizedUserName = adminEmail.ToUpperInvariant(),
                    FullName = "Demo Therapist"
                };

                var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createUserResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to seed admin user: {string.Join(", ", createUserResult.Errors.Select(x => x.Description))}");
                }

                await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
            }

            if (!await context.Articles.AnyAsync())
            {
                context.Articles.AddRange(
                    new Article
                    {
                        Id = Guid.NewGuid(),
                        Title = "Welcome to PhysioBook",
                        Content = "This is the seeded welcome article for the demo scaffold.",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow,
                        CreatedByUserId = adminUser.Id
                    },
                    new Article
                    {
                        Id = Guid.NewGuid(),
                        Title = "How online booking will fit here later",
                        Content = "Appointments, therapists and Stripe payments can be added as next vertical slices.",
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow,
                        CreatedByUserId = adminUser.Id
                    });

                await context.SaveChangesAsync();
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Database initialization failed.");
            throw;
        }
    }
}
