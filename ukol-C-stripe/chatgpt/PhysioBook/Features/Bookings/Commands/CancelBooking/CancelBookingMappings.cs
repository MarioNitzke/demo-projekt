using PhysioBook.Data.Entities;

namespace PhysioBook.Features.Bookings.Commands.CancelBooking;

public static class CancelBookingMappings
{
    public static CancelBookingCommand ToCommand(this CancelBookingRequest request, Guid id, string? userId, bool isAdmin)
    {
        return new CancelBookingCommand
        {
            Id = id,
            UserId = userId,
            IsAdmin = isAdmin
        };
    }

    public static CancelBookingResponse ToResponse(this Booking booking)
    {
        return new CancelBookingResponse(
            booking.Id,
            booking.Status.ToString(),
            booking.CancelledAt);
    }
}
