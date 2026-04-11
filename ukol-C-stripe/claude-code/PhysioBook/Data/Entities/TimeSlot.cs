using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PhysioBook.Data.Entities;

public class TimeSlot
{
    public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; }
}

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.DayOfWeek)
            .IsRequired();

        builder.Property(t => t.StartTime)
            .IsRequired();

        builder.Property(t => t.EndTime)
            .IsRequired();

        builder.Property(t => t.IsAvailable)
            .IsRequired();

        builder.HasIndex(t => new { t.DayOfWeek, t.IsAvailable });
    }
}
