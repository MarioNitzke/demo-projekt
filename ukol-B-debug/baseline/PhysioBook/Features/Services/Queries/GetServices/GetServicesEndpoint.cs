using PhysioBook.Exceptions;

namespace PhysioBook.Features.Services.Queries.GetServices;

public static class GetServicesEndpoint
{
    public static RouteGroupBuilder MapGetServicesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async ([AsParameters] GetServicesRequest request, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var query = request.ToQuery();

                var response = await mediator.SendAsync<GetServicesQuery, GetServicesResponse>(query, ct);

                return Results.Ok(response);
            });
        })
        .AllowAnonymous()
        .Produces<GetServicesResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("GetServices");

        return group;
    }
}
