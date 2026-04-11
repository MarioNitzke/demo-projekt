namespace PhysioBook.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommand : IQuery<CreateBookingResponse>
{
    public Guid ServiceId { get; set; }
    public DateTime StartTime { get; set; }
    public string? Notes { get; set; }
    public string? ClientId { get; set; }
}
