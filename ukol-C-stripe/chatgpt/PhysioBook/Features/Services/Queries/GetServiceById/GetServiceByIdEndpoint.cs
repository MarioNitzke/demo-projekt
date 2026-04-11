using PhysioBook.Exceptions;

namespace PhysioBook.Features.Services.Queries.GetServiceById;

public static class GetServiceByIdEndpoint
{
    public static RouteGroupBuilder MapGetServiceByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var query = new GetServiceByIdRequest(id).ToQuery();

                var response = await mediator.SendAsync<GetServiceByIdQuery, GetServiceByIdResponse>(query, ct);

                return Results.Ok(response);
            });
        })
        .AllowAnonymous()
        .Produces<GetServiceByIdResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetServiceById");

        return group;
    }
}
