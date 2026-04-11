namespace PhysioBook.Features.Bookings.Commands.CancelBooking;

public class CancelBookingCommand : IQuery<CancelBookingResponse>
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public bool IsAdmin { get; set; }
}
