using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhysioBook.Data.Enums;

namespace PhysioBook.Data.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public ApplicationUser Client { get; set; } = null!;
    public Service Service { get; set; } = null!;
}

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.ClientId)
            .IsRequired();

        builder.Property(b => b.StartTime)
            .IsRequired();

        builder.Property(b => b.EndTime)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.HasIndex(b => new { b.ServiceId, b.StartTime });

        builder.HasIndex(b => b.ClientId);

        builder.HasOne(b => b.Client)
            .WithMany()
            .HasForeignKey(b => b.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Service)
            .WithMany()
            .HasForeignKey(b => b.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
