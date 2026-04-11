using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PhysioBook.Data.Entities;

public class Service
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .IsRequired();

        builder.Property(s => s.Price)
            .HasPrecision(10, 2);

        builder.HasIndex(s => s.IsActive);
    }
}
