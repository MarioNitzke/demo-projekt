using PhysioBook.Exceptions;

namespace PhysioBook.Features.TimeSlots.Queries.GetTimeSlots;

public static class GetTimeSlotsEndpoint
{
    public static RouteGroupBuilder MapGetTimeSlotsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async ([AsParameters] GetTimeSlotsRequest request, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var query = request.ToQuery();

                var response = await mediator.SendAsync<GetTimeSlotsQuery, GetTimeSlotsResponse>(query, ct);

                return Results.Ok(response);
            });
        })
        .AllowAnonymous()
        .Produces<GetTimeSlotsResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("GetTimeSlots");

        return group;
    }
}
