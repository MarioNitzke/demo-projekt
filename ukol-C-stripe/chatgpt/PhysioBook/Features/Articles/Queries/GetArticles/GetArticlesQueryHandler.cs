using PhysioBook.Data;
using PhysioBook.Data.Shared;

namespace PhysioBook.Features.Articles.Queries.GetArticles;

public class GetArticlesQueryHandler : IQueryHandler<GetArticlesQuery, GetArticlesResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetArticlesQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetArticlesResponse> Handle(GetArticlesQuery query, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var articles = context.Articles
            .OrderByDescending(a => a.CreatedAt);

        var pagedList = await articles.ToPagedListAsync(query.PageNumber, query.PageSize, ct);

        return pagedList.ToResponse();
    }
}
