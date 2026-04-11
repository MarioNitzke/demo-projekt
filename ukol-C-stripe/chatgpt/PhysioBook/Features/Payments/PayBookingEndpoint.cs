using System.Security.Claims;
using PhysioBook.Configurations;
using PhysioBook.Data;
using PhysioBook.Data.Enums;

namespace PhysioBook.Features.Payments;

public static class PayBookingEndpoint
{
    public static RouteGroupBuilder MapPayBookingEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/pay", async (Guid id, IMediator mediator,
                   PhysioBookContext db, IConfiguration config,
                   HttpContext httpContext, CancellationToken ct) =>
        {
            // získání uživatele a oprávnění
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = httpContext.User.IsInRole("Admin");

            // načtení rezervace
            var booking = await db.Bookings
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == id, ct);
            if (booking == null) return Results.NotFound();

            if (booking.ClientId != userId && !isAdmin)
                return Results.Forbid();

            if (booking.PaymentStatus == PaymentStatus.Paid)
                return Results.BadRequest("Rezervace je již zaplacená.");

            // vytvoření Stripe session
            var stripeSettings = config.GetSection("Stripe").Get<StripeSettings>()!;
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                Mode = "payment",
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                    new()
                    {
                        Quantity = 1,
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            Currency = "czk",
                            UnitAmount = (long)(booking.Service.Price * 100),
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = booking.Service.Name
                            }
                        }
                    }
                },
                SuccessUrl = $"http://localhost:5173/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl  = $"http://localhost:5173/payment-cancel?booking_id={id}",
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            var service = new Stripe.Checkout.SessionService();
            var session = await service.CreateAsync(options);

            booking.StripeSessionId = session.Id;
            booking.StripePaymentIntentId = session.PaymentIntentId;
            booking.PaymentStatus = PaymentStatus.Unpaid;
            await db.SaveChangesAsync(ct);

            return Results.Ok(new { sessionId = session.Id, url = session.Url });
        })
        .RequireAuthorization()
        .Produces(200)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("PayBooking");
        return group;
    }
}
