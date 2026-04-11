using System.Security.Claims;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Bookings.Commands.CreateBooking;

public static class CreateBookingEndpoint
{
    public static RouteGroupBuilder MapCreateBookingEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateBookingRequest request, IMediator mediator, HttpContext httpContext, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();
                command.ClientId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var response = await mediator.SendAsync<CreateBookingCommand, CreateBookingResponse>(command, ct);

                return Results.Created($"/api/bookings/{response.Id}", response);
            });
        })
        .RequireAuthorization()
        .Produces<CreateBookingResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("CreateBooking");

        return group;
    }
}
