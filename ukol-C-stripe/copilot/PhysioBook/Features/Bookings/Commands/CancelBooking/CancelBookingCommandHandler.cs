using PhysioBook.Data;
using PhysioBook.Data.Enums;

namespace PhysioBook.Features.Bookings.Commands.CancelBooking;

public class CancelBookingCommandHandler : IQueryHandler<CancelBookingCommand, CancelBookingResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public CancelBookingCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<CancelBookingResponse> Handle(CancelBookingCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var booking = await context.Bookings.FindAsync(new object[] { command.Id }, ct)
            ?? throw new KeyNotFoundException("Booking not found");

        if (booking.ClientId != command.UserId && !command.IsAdmin)
            throw new UnauthorizedAccessException("You can only cancel your own bookings");

        booking.Status = BookingStatus.Cancelled;
        if (booking.PaymentStatus != PaymentStatus.Paid)
        {
            booking.PaymentStatus = PaymentStatus.Cancelled;
        }
        booking.CancelledAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);

        return booking.ToResponse();
    }
}
