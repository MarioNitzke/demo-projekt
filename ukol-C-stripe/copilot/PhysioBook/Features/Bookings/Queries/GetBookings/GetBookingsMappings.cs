using PhysioBook.Data.Entities;
using PhysioBook.Data.Shared;

namespace PhysioBook.Features.Bookings.Queries.GetBookings;

public static class GetBookingsMappings
{
    public static GetBookingsQuery ToQuery(this GetBookingsRequest request, string? userId, bool isAdmin)
    {
        return new GetBookingsQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            UserId = userId,
            IsAdmin = isAdmin
        };
    }

    public static GetBookingsResponse ToResponse(this PagedList<Booking> pagedList)
    {
        return new GetBookingsResponse(
            pagedList.Items.Select(b => b.ToItemResponse()).ToList(),
            pagedList.TotalCount,
            pagedList.PageNumber,
            pagedList.PageSize,
            pagedList.TotalPages);
    }

    public static GetBookingsItemResponse ToItemResponse(this Booking booking)
    {
        return new GetBookingsItemResponse(
            booking.Id,
            booking.ClientId,
            booking.ServiceId,
            booking.Service?.Name ?? string.Empty,
            booking.StartTime,
            booking.EndTime,
            booking.Status.ToString(),
            booking.PaymentStatus.ToString(),
            booking.StripeSessionId,
            booking.StripePaymentIntentId,
            booking.Notes,
            booking.CreatedAt);
    }
}
