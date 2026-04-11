using PhysioBook.Exceptions;

namespace PhysioBook.Features.Services.Commands.CreateService;

public static class CreateServiceEndpoint
{
    public static RouteGroupBuilder MapCreateServiceEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateServiceRequest request, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();

                var response = await mediator.SendAsync<CreateServiceCommand, CreateServiceResponse>(command, ct);

                return Results.Created($"/api/services/{response.Id}", response);
            });
        })
        .RequireAuthorization("Admin")
        .Produces<CreateServiceResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("CreateService");

        return group;
    }
}
