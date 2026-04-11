using System.Security.Claims;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Bookings.Queries.GetBookingById;

public static class GetBookingByIdEndpoint
{
    public static RouteGroupBuilder MapGetBookingByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, HttpContext httpContext, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = httpContext.User.IsInRole("Admin");

                var query = new GetBookingByIdRequest(id).ToQuery(userId, isAdmin);

                var response = await mediator.SendAsync<GetBookingByIdQuery, GetBookingByIdResponse>(query, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization()
        .Produces<GetBookingByIdResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetBookingById");

        return group;
    }
}
