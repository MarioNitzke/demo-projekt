using PhysioBook.Data;

namespace PhysioBook.Features.TimeSlots.Commands.UpdateTimeSlot;

public class UpdateTimeSlotCommandHandler : IQueryHandler<UpdateTimeSlotCommand, UpdateTimeSlotResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public UpdateTimeSlotCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<UpdateTimeSlotResponse> Handle(UpdateTimeSlotCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var timeSlot = await context.TimeSlots.FindAsync(new object[] { command.Id }, ct)
            ?? throw new KeyNotFoundException($"TimeSlot with Id '{command.Id}' was not found.");

        command.ApplyTo(timeSlot);
        await context.SaveChangesAsync(ct);

        return timeSlot.ToResponse();
    }
}
