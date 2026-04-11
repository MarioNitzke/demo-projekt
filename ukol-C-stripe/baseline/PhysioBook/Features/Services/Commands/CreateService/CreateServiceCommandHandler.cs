using PhysioBook.Data;

namespace PhysioBook.Features.Services.Commands.CreateService;

public class CreateServiceCommandHandler : IQueryHandler<CreateServiceCommand, CreateServiceResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public CreateServiceCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<CreateServiceResponse> Handle(CreateServiceCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var service = command.ToEntity();

        context.Services.Add(service);
        await context.SaveChangesAsync(ct);

        return service.ToResponse();
    }
}
