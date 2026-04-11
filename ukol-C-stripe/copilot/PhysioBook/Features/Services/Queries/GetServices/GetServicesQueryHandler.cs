using PhysioBook.Data;
using PhysioBook.Data.Shared;

namespace PhysioBook.Features.Services.Queries.GetServices;

public class GetServicesQueryHandler : IQueryHandler<GetServicesQuery, GetServicesResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetServicesQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetServicesResponse> Handle(GetServicesQuery query, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var services = context.Services
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name);

        var pagedList = await services.ToPagedListAsync(query.PageNumber, query.PageSize, ct);

        return pagedList.ToResponse();
    }
}
