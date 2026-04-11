using Microsoft.Extensions.Options;
using PhysioBook.Configurations;
using PhysioBook.Data;
using PhysioBook.Data.Enums;
using Stripe;
using Stripe.Checkout;

namespace PhysioBook.Features.Webhooks.Stripe;

public static class StripeWebhookEndpoint
{
    public static RouteGroupBuilder MapStripeWebhookEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/stripe", async (HttpContext httpContext, IDbContextFactory<PhysioBookContext> contextFactory, IOptions<StripeSettings> stripeOptions, CancellationToken ct) =>
        {
            var webhookSecret = stripeOptions.Value.WebhookSecret;
            if (string.IsNullOrWhiteSpace(webhookSecret))
            {
                return Results.Problem("Stripe webhook secret is not configured", statusCode: StatusCodes.Status500InternalServerError);
            }

            string payload;
            using (var reader = new StreamReader(httpContext.Request.Body))
            {
                payload = await reader.ReadToEndAsync(ct);
            }

            var stripeSignature = httpContext.Request.Headers["Stripe-Signature"].ToString();
            if (string.IsNullOrWhiteSpace(stripeSignature))
            {
                return Results.BadRequest("Missing Stripe-Signature header");
            }

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(payload, stripeSignature, webhookSecret);
            }
            catch (StripeException ex)
            {
                return Results.BadRequest($"Invalid signature: {ex.Message}");
            }

            if (stripeEvent.Data.Object is not Session session)
            {
                return Results.Ok();
            }

            using var context = await contextFactory.CreateDbContextAsync(ct);

            var booking = await FindBookingAsync(context, session, ct);
            if (booking == null)
            {
                return Results.Ok();
            }

            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                    booking.PaymentStatus = PaymentStatus.Paid;
                    booking.Status = BookingStatus.Paid;
                    booking.StripePaymentIntentId = session.PaymentIntentId;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync(ct);
                    break;

                case EventTypes.CheckoutSessionExpired:
                    if (booking.PaymentStatus != PaymentStatus.Paid)
                    {
                        booking.PaymentStatus = PaymentStatus.Expired;
                        booking.Status = BookingStatus.Cancelled;
                        booking.CancelledAt = DateTime.UtcNow;
                        booking.UpdatedAt = DateTime.UtcNow;
                        await context.SaveChangesAsync(ct);
                    }
                    break;
            }

            return Results.Ok();
        })
        .AllowAnonymous()
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("StripeWebhook");

        return group;
    }

    private static async Task<Data.Entities.Booking?> FindBookingAsync(PhysioBookContext context, Session session, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(session.Id))
        {
            var bySession = await context.Bookings.FirstOrDefaultAsync(b => b.StripeSessionId == session.Id, ct);
            if (bySession != null)
            {
                return bySession;
            }
        }

        if (session.Metadata != null &&
            session.Metadata.TryGetValue("bookingId", out var rawBookingId) &&
            Guid.TryParse(rawBookingId, out var bookingId))
        {
            return await context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, ct);
        }

        return null;
    }
}

