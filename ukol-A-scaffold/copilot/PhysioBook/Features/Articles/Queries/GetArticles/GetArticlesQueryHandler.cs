using PhysioBook.Data.Shared;

namespace PhysioBook.Features.Articles.Queries.GetArticles;

public sealed class GetArticlesQueryHandler : IQueryHandler<GetArticlesQuery, GetArticlesResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetArticlesQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetArticlesResponse> Handle(GetArticlesQuery query, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var paged = await context.Articles
            .OrderByDescending(x => x.CreatedAtUtc)
            .AsNoTracking()
            .ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return paged.ToResponse();
    }
}

