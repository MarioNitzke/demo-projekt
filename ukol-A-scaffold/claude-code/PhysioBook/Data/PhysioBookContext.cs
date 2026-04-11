using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PhysioBook.Data.Entities;

namespace PhysioBook.Data;

public class PhysioBookContext : IdentityDbContext<ApplicationUser>
{
    public PhysioBookContext(DbContextOptions<PhysioBookContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles => Set<Article>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(PhysioBookContext).Assembly);
    }
}
