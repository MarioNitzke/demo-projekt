using Microsoft.Extensions.Options;
using PhysioBook.Configurations;
using PhysioBook.Data;
using PhysioBook.Data.Enums;
using Stripe.Checkout;

namespace PhysioBook.Features.Bookings.Commands.CreateCheckoutSession;

public class CreateCheckoutSessionCommandHandler : IQueryHandler<CreateCheckoutSessionCommand, CreateCheckoutSessionResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;
    private readonly StripeSettings _stripeSettings;

    public CreateCheckoutSessionCommandHandler(
        IDbContextFactory<PhysioBookContext> contextFactory,
        IOptions<StripeSettings> stripeSettings)
    {
        _contextFactory = contextFactory;
        _stripeSettings = stripeSettings.Value;
    }

    public async Task<CreateCheckoutSessionResponse> Handle(CreateCheckoutSessionCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var booking = await context.Bookings
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b => b.Id == command.BookingId, ct)
            ?? throw new KeyNotFoundException("Booking not found");

        if (booking.ClientId != command.ClientId)
            throw new UnauthorizedAccessException("You can only pay for your own bookings");

        if (booking.Status == BookingStatus.Cancelled)
            throw new ArgumentException("Cannot pay for a cancelled booking");

        if (booking.PaymentStatus == PaymentStatus.Paid)
            throw new ArgumentException("Booking is already paid");

        var sessionService = new SessionService();
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = booking.Service.Price * 100, // Stripe expects amount in cents
                        Currency = "czk",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = booking.Service.Name,
                            Description = $"Rezervace: {booking.StartTime:dd.MM.yyyy HH:mm} - {booking.EndTime:HH:mm}"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = _stripeSettings.SuccessUrl,
            CancelUrl = _stripeSettings.CancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", booking.Id.ToString() }
            }
        };

        var session = await sessionService.CreateAsync(options, cancellationToken: ct);

        booking.StripeSessionId = session.Id;
        booking.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);

        return new CreateCheckoutSessionResponse(session.Url);
    }
}
