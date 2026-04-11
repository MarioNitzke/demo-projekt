using PhysioBook.Exceptions;

namespace PhysioBook.Features.TimeSlots.Commands.DeleteTimeSlot;

public static class DeleteTimeSlotEndpoint
{
    public static RouteGroupBuilder MapDeleteTimeSlotEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = new DeleteTimeSlotRequest(id).ToCommand();

                var response = await mediator.SendAsync<DeleteTimeSlotCommand, DeleteTimeSlotResponse>(command, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization("Admin")
        .Produces<DeleteTimeSlotResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("DeleteTimeSlot");

        return group;
    }
}
