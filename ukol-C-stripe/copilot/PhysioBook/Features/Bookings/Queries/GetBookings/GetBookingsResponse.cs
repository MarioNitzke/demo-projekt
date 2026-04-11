namespace PhysioBook.Features.Bookings.Queries.GetBookings;

public record GetBookingsResponse(
    List<GetBookingsItemResponse> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

public record GetBookingsItemResponse(
    Guid Id,
    string ClientId,
    Guid ServiceId,
    string ServiceName,
    DateTime StartTime,
    DateTime EndTime,
    string Status,
    string PaymentStatus,
    string? StripeSessionId,
    string? StripePaymentIntentId,
    string? Notes,
    DateTime CreatedAt);
