namespace PhysioBook.Features.Bookings.Queries.GetBookings;

public record GetBookingsRequest(int PageNumber = 1, int PageSize = 10);
