using System.Security.Claims;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Bookings.Commands.CreateCheckoutSession;

public static class CreateCheckoutSessionEndpoint
{
    public static RouteGroupBuilder MapCreateCheckoutSessionEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{bookingId:guid}/checkout", async (Guid bookingId, IMediator mediator, HttpContext httpContext, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = new CreateCheckoutSessionCommand
                {
                    BookingId = bookingId,
                    UserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    IsAdmin = httpContext.User.IsInRole("Admin")
                };

                var response = await mediator.SendAsync<CreateCheckoutSessionCommand, CreateCheckoutSessionResponse>(command, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization()
        .Produces<CreateCheckoutSessionResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("CreateCheckoutSession");

        return group;
    }
}

