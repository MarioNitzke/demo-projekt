using PhysioBook.Data;

namespace PhysioBook.Features.Services.Commands.UpdateService;

public class UpdateServiceCommandHandler : IQueryHandler<UpdateServiceCommand, UpdateServiceResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public UpdateServiceCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<UpdateServiceResponse> Handle(UpdateServiceCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var service = await context.Services.FindAsync(new object[] { command.Id }, ct)
            ?? throw new KeyNotFoundException($"Service with Id '{command.Id}' was not found.");

        command.ApplyTo(service);
        await context.SaveChangesAsync(ct);

        return service.ToResponse();
    }
}
