using PhysioBook.Data;
using PhysioBook.Data.Enums;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler : IQueryHandler<CreateBookingCommand, CreateBookingResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public CreateBookingCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<CreateBookingResponse> Handle(CreateBookingCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var service = await context.Services.FindAsync(new object[] { command.ServiceId }, ct)
            ?? throw new KeyNotFoundException("Service not found");

        var endTime = command.StartTime.AddMinutes(service.DurationMinutes);

        if (command.StartTime <= DateTime.UtcNow)
            throw new ArgumentException("Booking must be in the future");

        var dayOfWeek = command.StartTime.DayOfWeek;
        var startTimeOnly = TimeOnly.FromDateTime(command.StartTime);
        var endTimeOnly = TimeOnly.FromDateTime(endTime);

        var hasValidSlot = await context.TimeSlots.AnyAsync(ts =>
            ts.DayOfWeek == dayOfWeek &&
            ts.IsAvailable &&
            ts.StartTime <= startTimeOnly &&
            ts.EndTime >= endTimeOnly, ct);

        if (!hasValidSlot)
            throw new ArgumentException("No available time slot for the requested time");

        // Recheck for overlapping bookings
        var hasOverlap = await context.Bookings.AnyAsync(b =>
            b.ServiceId == command.ServiceId &&
            b.StartTime < endTime &&
            b.EndTime > command.StartTime &&
            b.Status != BookingStatus.Cancelled, ct);

        if (hasOverlap)
            throw new BookingConflictException("The requested time slot is already booked");

        var booking = command.ToEntity(endTime);
        context.Bookings.Add(booking);
        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return booking.ToResponse();
    }
}
