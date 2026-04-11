namespace PhysioBook.Features.Bookings.Queries.GetBookings;

public class GetBookingsQuery : IQuery<GetBookingsResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? UserId { get; set; }
    public bool IsAdmin { get; set; }
}
