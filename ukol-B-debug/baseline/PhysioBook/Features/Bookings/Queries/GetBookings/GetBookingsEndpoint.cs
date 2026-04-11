using System.Security.Claims;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Bookings.Queries.GetBookings;

public static class GetBookingsEndpoint
{
    public static RouteGroupBuilder MapGetBookingsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async ([AsParameters] GetBookingsRequest request, IMediator mediator, HttpContext httpContext, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = httpContext.User.IsInRole("Admin");

                var query = request.ToQuery(userId, isAdmin);

                var response = await mediator.SendAsync<GetBookingsQuery, GetBookingsResponse>(query, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization()
        .Produces<GetBookingsResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("GetBookings");

        return group;
    }
}
