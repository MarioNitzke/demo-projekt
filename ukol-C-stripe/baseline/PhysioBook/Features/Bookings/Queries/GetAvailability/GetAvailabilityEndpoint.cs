using PhysioBook.Exceptions;

namespace PhysioBook.Features.Bookings.Queries.GetAvailability;

public static class GetAvailabilityEndpoint
{
    public static RouteGroupBuilder MapGetAvailabilityEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/availability", async ([AsParameters] GetAvailabilityRequest request, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var query = request.ToQuery();

                var response = await mediator.SendAsync<GetAvailabilityQuery, GetAvailabilityResponse>(query, ct);

                return Results.Ok(response);
            });
        })
        .AllowAnonymous()
        .Produces<GetAvailabilityResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("GetAvailability");

        return group;
    }
}
