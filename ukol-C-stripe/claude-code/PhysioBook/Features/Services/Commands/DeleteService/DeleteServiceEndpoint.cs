using PhysioBook.Exceptions;

namespace PhysioBook.Features.Services.Commands.DeleteService;

public static class DeleteServiceEndpoint
{
    public static RouteGroupBuilder MapDeleteServiceEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = new DeleteServiceRequest(id).ToCommand();

                var response = await mediator.SendAsync<DeleteServiceCommand, DeleteServiceResponse>(command, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization("Admin")
        .Produces<DeleteServiceResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("DeleteService");

        return group;
    }
}
