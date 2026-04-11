using System.Text;
using Microsoft.Extensions.Options;
using PhysioBook.Configurations;
using PhysioBook.Data;
using PhysioBook.Data.Enums;
using Stripe;

namespace PhysioBook.Features.Payments;

public static class StripeWebhookEndpoint
{
    public static void MapStripeWebhookEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/webhooks/stripe", async (HttpContext context, PhysioBookContext db, IOptions<StripeSettings> options, ILogger<PhysioBookContext> logger) =>
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            var signature = context.Request.Headers["Stripe-Signature"].ToString();
            var secret = options.Value.WebhookSecret;

            logger.LogInformation("Received Stripe webhook: {Signature}", signature);

            Stripe.Event stripeEvent;
            try
            {
                stripeEvent = Stripe.EventUtility.ConstructEvent(body, signature, secret, 300, throwOnApiVersionMismatch: false);
            }
            catch (StripeException ex)
            {
                logger.LogError("Stripe webhook verification failed: {Message}", ex.Message);
                return Results.BadRequest();
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                var sessionId = session != null ? session.Id : null;
                var booking = await db.Bookings
                    .FirstOrDefaultAsync(b => b.StripeSessionId == sessionId);
                if (booking != null)
                {
                    booking.PaymentStatus = PaymentStatus.Paid;
                    booking.Status = BookingStatus.Confirmed;
                    await db.SaveChangesAsync();
                }
            }
            else if (stripeEvent.Type == "checkout.session.expired")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                var sessionId = session != null ? session.Id : null;
                var booking = await db.Bookings
                    .FirstOrDefaultAsync(b => b.StripeSessionId == sessionId);
                if (booking != null)
                {
                    booking.PaymentStatus = PaymentStatus.Expired;
                    booking.Status = BookingStatus.Cancelled;
                    booking.CancelledAt = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                }
            }

            return Results.Ok();
        }).AllowAnonymous();
    }
}
