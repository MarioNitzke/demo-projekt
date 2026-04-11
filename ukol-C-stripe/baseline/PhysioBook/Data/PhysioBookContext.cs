using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PhysioBook.Data.Entities;

namespace PhysioBook.Data;

public class PhysioBookContext : IdentityDbContext<ApplicationUser>
{
    public PhysioBookContext(DbContextOptions<PhysioBookContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(PhysioBookContext).Assembly);
    }
}
