namespace PhysioBook.Features.Bookings.Commands.CreateCheckoutSession;

public record CreateCheckoutSessionResponse(
    Guid BookingId,
    string StripeSessionId,
    string CheckoutUrl);

