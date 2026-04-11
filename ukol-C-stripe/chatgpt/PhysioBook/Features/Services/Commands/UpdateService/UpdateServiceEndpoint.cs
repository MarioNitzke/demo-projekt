using PhysioBook.Exceptions;

namespace PhysioBook.Features.Services.Commands.UpdateService;

public static class UpdateServiceEndpoint
{
    public static RouteGroupBuilder MapUpdateServiceEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (Guid id, UpdateServiceRequest request, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand(id);

                var response = await mediator.SendAsync<UpdateServiceCommand, UpdateServiceResponse>(command, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization("Admin")
        .Produces<UpdateServiceResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("UpdateService");

        return group;
    }
}
