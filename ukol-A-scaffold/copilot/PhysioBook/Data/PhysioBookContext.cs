namespace PhysioBook.Data;

public sealed class PhysioBookContext : IdentityDbContext<AppUser>
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

