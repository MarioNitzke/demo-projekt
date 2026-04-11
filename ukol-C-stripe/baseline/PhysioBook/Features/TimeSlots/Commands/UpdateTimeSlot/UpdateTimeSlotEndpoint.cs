using PhysioBook.Exceptions;

namespace PhysioBook.Features.TimeSlots.Commands.UpdateTimeSlot;

public static class UpdateTimeSlotEndpoint
{
    public static RouteGroupBuilder MapUpdateTimeSlotEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (Guid id, UpdateTimeSlotRequest request, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand(id);

                var response = await mediator.SendAsync<UpdateTimeSlotCommand, UpdateTimeSlotResponse>(command, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization("Admin")
        .Produces<UpdateTimeSlotResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("UpdateTimeSlot");

        return group;
    }
}
