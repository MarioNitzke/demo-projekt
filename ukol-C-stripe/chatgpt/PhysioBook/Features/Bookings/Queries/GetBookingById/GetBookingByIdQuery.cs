namespace PhysioBook.Features.Bookings.Queries.GetBookingById;

public class GetBookingByIdQuery : IQuery<GetBookingByIdResponse>
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public bool IsAdmin { get; set; }
}
