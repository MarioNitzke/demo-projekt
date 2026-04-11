using PhysioBook.Data;

namespace PhysioBook.Features.TimeSlots.Commands.DeleteTimeSlot;

public class DeleteTimeSlotCommandHandler : IQueryHandler<DeleteTimeSlotCommand, DeleteTimeSlotResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public DeleteTimeSlotCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DeleteTimeSlotResponse> Handle(DeleteTimeSlotCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var timeSlot = await context.TimeSlots.FindAsync(new object[] { command.Id }, ct)
            ?? throw new KeyNotFoundException($"TimeSlot with Id '{command.Id}' was not found.");

        context.TimeSlots.Remove(timeSlot);
        await context.SaveChangesAsync(ct);

        return new DeleteTimeSlotResponse(true);
    }
}
