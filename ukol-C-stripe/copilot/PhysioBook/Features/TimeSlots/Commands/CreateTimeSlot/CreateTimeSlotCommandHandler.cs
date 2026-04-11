using PhysioBook.Data;

namespace PhysioBook.Features.TimeSlots.Commands.CreateTimeSlot;

public class CreateTimeSlotCommandHandler : IQueryHandler<CreateTimeSlotCommand, CreateTimeSlotResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public CreateTimeSlotCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<CreateTimeSlotResponse> Handle(CreateTimeSlotCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var timeSlot = command.ToEntity();

        context.TimeSlots.Add(timeSlot);
        await context.SaveChangesAsync(ct);

        return timeSlot.ToResponse();
    }
}
