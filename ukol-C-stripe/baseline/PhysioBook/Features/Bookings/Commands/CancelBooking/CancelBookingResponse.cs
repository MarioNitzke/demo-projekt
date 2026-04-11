namespace PhysioBook.Features.Bookings.Commands.CancelBooking;

public record CancelBookingResponse(Guid Id, string Status, DateTime? CancelledAt);
