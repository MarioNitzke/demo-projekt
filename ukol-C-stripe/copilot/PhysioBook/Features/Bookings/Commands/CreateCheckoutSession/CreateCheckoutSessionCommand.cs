namespace PhysioBook.Features.Bookings.Commands.CreateCheckoutSession;

public class CreateCheckoutSessionCommand : IQuery<CreateCheckoutSessionResponse>
{
    public Guid BookingId { get; set; }
    public string? UserId { get; set; }
    public bool IsAdmin { get; set; }
}

