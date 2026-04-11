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
        IOptions<StripeSettings> stripeOptions)
    {
        _contextFactory = contextFactory;
        _stripeSettings = stripeOptions.Value;
    }

    public async Task<CreateCheckoutSessionResponse> Handle(CreateCheckoutSessionCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_stripeSettings.SecretKey))
            throw new InvalidOperationException("Stripe SecretKey is not configured");

        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var booking = await context.Bookings
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b => b.Id == command.BookingId, ct)
            ?? throw new KeyNotFoundException("Booking not found");

        if (booking.ClientId != command.UserId && !command.IsAdmin)
            throw new UnauthorizedAccessException("You can only pay your own bookings");

        if (booking.Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Cancelled booking cannot be paid");

        if (booking.PaymentStatus == PaymentStatus.Paid)
            throw new InvalidOperationException("Booking is already paid");

        var frontendBaseUrl = _stripeSettings.FrontendBaseUrl.TrimEnd('/');
        var sessionService = new SessionService();

        var session = await sessionService.CreateAsync(new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{frontendBaseUrl}/my-bookings?payment=success&bookingId={booking.Id}",
            CancelUrl = $"{frontendBaseUrl}/my-bookings?payment=cancel&bookingId={booking.Id}",
            ClientReferenceId = booking.Id.ToString(),
            Metadata = new Dictionary<string, string>
            {
                ["bookingId"] = booking.Id.ToString()
            },
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = _stripeSettings.Currency,
                        UnitAmount = (long)decimal.Round(booking.Service.Price * 100m),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = booking.Service.Name
                        }
                    }
                }
            ]
        }, cancellationToken: ct);

        booking.StripeSessionId = session.Id;
        booking.StripePaymentIntentId = session.PaymentIntentId;
        booking.PaymentStatus = PaymentStatus.Pending;
        booking.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return new CreateCheckoutSessionResponse(
            booking.Id,
            session.Id,
            session.Url ?? string.Empty);
    }
}

