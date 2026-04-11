using PhysioBook.Data.Entities;

namespace PhysioBook.Features.Bookings.Queries.GetBookingById;

public static class GetBookingByIdMappings
{
    public static GetBookingByIdQuery ToQuery(this GetBookingByIdRequest request, string? userId, bool isAdmin)
    {
        return new GetBookingByIdQuery
        {
            Id = request.Id,
            UserId = userId,
            IsAdmin = isAdmin
        };
    }

    public static GetBookingByIdResponse ToResponse(this Booking booking)
    {
        return new GetBookingByIdResponse(
            booking.Id,
            booking.ClientId,
            booking.ServiceId,
            booking.Service?.Name ?? string.Empty,
            booking.StartTime,
            booking.EndTime,
            booking.Status.ToString(),
            booking.Notes,
            booking.CreatedAt,
            booking.UpdatedAt,
            booking.CancelledAt);
    }
}
