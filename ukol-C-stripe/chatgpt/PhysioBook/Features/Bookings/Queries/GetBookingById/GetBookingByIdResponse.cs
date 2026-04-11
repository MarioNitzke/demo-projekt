namespace PhysioBook.Features.Bookings.Queries.GetBookingById;

public record GetBookingByIdResponse(
    Guid Id,
    string ClientId,
    Guid ServiceId,
    string ServiceName,
    DateTime StartTime,
    DateTime EndTime,
    string Status,
    string PaymentStatus,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? CancelledAt);
