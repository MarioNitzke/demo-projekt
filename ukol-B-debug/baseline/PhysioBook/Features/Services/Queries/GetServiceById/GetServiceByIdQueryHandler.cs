using PhysioBook.Data;

namespace PhysioBook.Features.Services.Queries.GetServiceById;

public class GetServiceByIdQueryHandler : IQueryHandler<GetServiceByIdQuery, GetServiceByIdResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetServiceByIdQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetServiceByIdResponse> Handle(GetServiceByIdQuery query, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var service = await context.Services.FindAsync(new object[] { query.Id }, ct)
            ?? throw new KeyNotFoundException($"Service with Id '{query.Id}' was not found.");

        return service.ToResponse();
    }
}
