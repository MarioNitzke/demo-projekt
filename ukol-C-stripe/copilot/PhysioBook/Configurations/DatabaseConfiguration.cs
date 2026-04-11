using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Auth;
using PhysioBook.Data.Entities;
using PhysioBook.Data.Enums;

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

            await SeedClientUserAsync(userManager);
            await SeedServicesAsync(context);
            await SeedTimeSlotsAsync(context);
            await SeedBookingsAsync(context, userManager);
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

    private static async Task SeedClientUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string clientEmail = "klient@physiobook.cz";
        const string clientPassword = "Klient123!";

        var existingClient = await userManager.FindByEmailAsync(clientEmail);
        if (existingClient != null) return;

        var clientUser = new ApplicationUser
        {
            UserName = clientEmail,
            Email = clientEmail,
            FirstName = "Karel",
            LastName = "Klient",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(clientUser, clientPassword);
    }

    private static async Task SeedServicesAsync(Data.PhysioBookContext context)
    {
        if (await context.Services.AnyAsync()) return;

        context.Services.AddRange(
            new Service { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Masáž", Description = "Klasická masáž celého těla", DurationMinutes = 60, Price = 800m, IsActive = true },
            new Service { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Rehabilitace", Description = "Individuální rehabilitační cvičení", DurationMinutes = 45, Price = 600m, IsActive = true },
            new Service { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Konzultace", Description = "Vstupní konzultace a diagnostika", DurationMinutes = 30, Price = 400m, IsActive = true },
            new Service { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Tejpování", Description = "Aplikace kineziotejpu", DurationMinutes = 20, Price = 300m, IsActive = true }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedTimeSlotsAsync(Data.PhysioBookContext context)
    {
        if (await context.TimeSlots.AnyAsync()) return;

        var slots = new List<TimeSlot>();
        for (var day = DayOfWeek.Monday; day <= DayOfWeek.Friday; day++)
        {
            slots.Add(new TimeSlot { Id = Guid.NewGuid(), DayOfWeek = day, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(12, 0), IsAvailable = true });
            slots.Add(new TimeSlot { Id = Guid.NewGuid(), DayOfWeek = day, StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(17, 0), IsAvailable = true });
        }
        context.TimeSlots.AddRange(slots);
        await context.SaveChangesAsync();
    }

    private static DateTime GetNextWeekday(DayOfWeek day)
    {
        var today = DateTime.UtcNow.Date;
        var daysUntil = ((int)day - (int)today.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7; // always next week
        return today.AddDays(daysUntil);
    }

    private static async Task SeedBookingsAsync(Data.PhysioBookContext context, UserManager<ApplicationUser> userManager)
    {
        if (await context.Bookings.AnyAsync()) return;

        var admin = await userManager.FindByEmailAsync("admin@physiobook.cz");
        if (admin == null) return;

        var nextMonday = GetNextWeekday(DayOfWeek.Monday);
        var nextTuesday = GetNextWeekday(DayOfWeek.Tuesday);

        context.Bookings.AddRange(
            new Booking
            {
                Id = Guid.NewGuid(), ClientId = admin.Id,
                ServiceId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                StartTime = nextMonday.AddHours(9), EndTime = nextMonday.AddHours(10),
                Status = BookingStatus.Confirmed, PaymentStatus = PaymentStatus.Paid, CreatedAt = DateTime.UtcNow
            },
            new Booking
            {
                Id = Guid.NewGuid(), ClientId = admin.Id,
                ServiceId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                StartTime = nextMonday.AddHours(14), EndTime = nextMonday.AddHours(14).AddMinutes(45),
                Status = BookingStatus.Confirmed, PaymentStatus = PaymentStatus.Paid, CreatedAt = DateTime.UtcNow
            },
            new Booking
            {
                Id = Guid.NewGuid(), ClientId = admin.Id,
                ServiceId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                StartTime = nextTuesday.AddHours(10), EndTime = nextTuesday.AddHours(10).AddMinutes(30),
                Status = BookingStatus.Cancelled, PaymentStatus = PaymentStatus.Cancelled, CreatedAt = DateTime.UtcNow,
                CancelledAt = DateTime.UtcNow
            }
        );
        await context.SaveChangesAsync();
    }
}
