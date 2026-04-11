namespace PhysioBook.Features.Bookings.Commands.CreateBooking;

public record CreateBookingResponse(
    Guid Id,
    string ClientId,
    Guid ServiceId,
    DateTime StartTime,
    DateTime EndTime,
    string Status,
    string? Notes,
    DateTime CreatedAt);
