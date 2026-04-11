namespace PhysioBook.Features.Bookings.Commands.CreateBooking;

public record CreateBookingRequest(Guid ServiceId, DateTime StartTime, string? Notes);
