using System.Security.Claims;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Bookings.Commands.CreateCheckoutSession;

public static class CreateCheckoutSessionEndpoint
{
    public static RouteGroupBuilder MapCreateCheckoutSessionEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/create-checkout-session", async (Guid id, IMediator mediator, HttpContext httpContext, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = new CreateCheckoutSessionCommand
                {
                    BookingId = id,
                    ClientId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                };

                var response = await mediator.SendAsync<CreateCheckoutSessionCommand, CreateCheckoutSessionResponse>(command, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization()
        .Produces<CreateCheckoutSessionResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("CreateCheckoutSession");

        return group;
    }
}
