using Microsoft.Extensions.Options;
using PhysioBook.Configurations;
using PhysioBook.Data;
using PhysioBook.Data.Enums;
using Stripe;

namespace PhysioBook.Features.Webhooks;

public static class StripeWebhookEndpoint
{
    public static void MapStripeWebhookEndpoint(this WebApplication app)
    {
        app.MapPost("/api/webhooks/stripe", async (
            HttpContext httpContext,
            IDbContextFactory<PhysioBookContext> contextFactory,
            IOptions<StripeSettings> stripeSettings,
            ILogger<Program> logger) =>
        {
            var json = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
            var webhookSecret = stripeSettings.Value.WebhookSecret;
            var signature = httpContext.Request.Headers["Stripe-Signature"].ToString();

            if (string.IsNullOrEmpty(signature))
            {
                logger.LogWarning("Stripe webhook received without Stripe-Signature header");
                return Results.BadRequest("Missing Stripe-Signature header");
            }

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    webhookSecret,
                    throwOnApiVersionMismatch: false);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Stripe webhook signature verification failed: {Message}", ex.Message);
                return Results.BadRequest("Invalid signature");
            }

            logger.LogInformation("Stripe webhook received: {EventType} ({EventId})", stripeEvent.Type, stripeEvent.Id);

            using var context = await contextFactory.CreateDbContextAsync();

            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if (session == null)
                    {
                        logger.LogWarning("Could not deserialize checkout session from event {EventId}", stripeEvent.Id);
                        break;
                    }

                    logger.LogInformation("Checkout session completed: {SessionId}, metadata: {Metadata}",
                        session.Id, string.Join(", ", session.Metadata?.Select(kv => $"{kv.Key}={kv.Value}") ?? []));

                    var bookingIdStr = session.Metadata?.GetValueOrDefault("bookingId");
                    if (bookingIdStr == null || !Guid.TryParse(bookingIdStr, out var bookingId))
                    {
                        logger.LogWarning("No valid bookingId in session metadata");
                        break;
                    }

                    var booking = await context.Bookings.FindAsync(bookingId);
                    if (booking == null)
                    {
                        logger.LogWarning("Booking {BookingId} not found", bookingId);
                        break;
                    }

                    booking.PaymentStatus = PaymentStatus.Paid;
                    booking.StripePaymentIntentId = session.PaymentIntentId;
                    booking.Status = BookingStatus.Confirmed;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();

                    logger.LogInformation("Payment completed for booking {BookingId}, PaymentIntent: {PaymentIntentId}",
                        bookingId, session.PaymentIntentId);
                    break;
                }

                case EventTypes.CheckoutSessionExpired:
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if (session == null) break;

                    var bookingIdStr = session.Metadata?.GetValueOrDefault("bookingId");
                    if (bookingIdStr == null || !Guid.TryParse(bookingIdStr, out var bookingId)) break;

                    var booking = await context.Bookings.FindAsync(bookingId);
                    if (booking == null) break;

                    booking.PaymentStatus = PaymentStatus.Expired;
                    booking.Status = BookingStatus.Cancelled;
                    booking.CancelledAt = DateTime.UtcNow;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();

                    logger.LogInformation("Payment expired for booking {BookingId}, booking cancelled", bookingId);
                    break;
                }

                default:
                    logger.LogDebug("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                    break;
            }

            return Results.Ok();
        })
        .AllowAnonymous()
        .WithTags("Webhooks")
        .WithName("StripeWebhook");
    }
}
