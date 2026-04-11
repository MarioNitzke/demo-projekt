using System.Security.Claims;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Bookings.Commands.CancelBooking;

public static class CancelBookingEndpoint
{
    public static RouteGroupBuilder MapCancelBookingEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/cancel", async (Guid id, IMediator mediator, HttpContext httpContext, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = httpContext.User.IsInRole("Admin");

                var command = new CancelBookingRequest(id).ToCommand(id, userId, isAdmin);

                var response = await mediator.SendAsync<CancelBookingCommand, CancelBookingResponse>(command, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization()
        .Produces<CancelBookingResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("CancelBooking");

        return group;
    }
}
