using PhysioBook.Data;

namespace PhysioBook.Features.Services.Commands.DeleteService;

public class DeleteServiceCommandHandler : IQueryHandler<DeleteServiceCommand, DeleteServiceResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public DeleteServiceCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DeleteServiceResponse> Handle(DeleteServiceCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var service = await context.Services.FindAsync(new object[] { command.Id }, ct)
            ?? throw new KeyNotFoundException($"Service with Id '{command.Id}' was not found.");

        service.IsActive = false;
        await context.SaveChangesAsync(ct);

        return new DeleteServiceResponse(true);
    }
}
