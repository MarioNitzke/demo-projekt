namespace PhysioBook.Features.Bookings.Commands.CreateCheckoutSession;

public class CreateCheckoutSessionCommand : IQuery<CreateCheckoutSessionResponse>
{
    public Guid BookingId { get; set; }
    public string? ClientId { get; set; }
}
