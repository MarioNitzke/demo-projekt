using PhysioBook.Exceptions;

namespace PhysioBook.Features.TimeSlots.Commands.CreateTimeSlot;

public static class CreateTimeSlotEndpoint
{
    public static RouteGroupBuilder MapCreateTimeSlotEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateTimeSlotRequest request, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();

                var response = await mediator.SendAsync<CreateTimeSlotCommand, CreateTimeSlotResponse>(command, ct);

                return Results.Created($"/api/timeslots/{response.Id}", response);
            });
        })
        .RequireAuthorization("Admin")
        .Produces<CreateTimeSlotResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("CreateTimeSlot");

        return group;
    }
}
