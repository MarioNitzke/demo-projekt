namespace PhysioBook.Api.Data;

public sealed class PhysioBookContext(DbContextOptions<PhysioBookContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole, string>(options)
{
    public DbSet<Article> Articles => Set<Article>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(PhysioBookContext).Assembly);
    }
}
