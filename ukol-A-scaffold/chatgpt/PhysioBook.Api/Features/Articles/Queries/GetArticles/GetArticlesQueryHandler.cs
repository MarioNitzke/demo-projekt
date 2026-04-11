using PhysioBook.Api.Data.Shared;

namespace PhysioBook.Api.Features.Articles.Queries.GetArticles;

public sealed class GetArticlesQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    : IQueryHandler<GetArticlesQuery, GetArticlesResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory = contextFactory;

    public async Task<GetArticlesResponse> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = context.Articles
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(x => x.Title.ToLower().Contains(searchTerm) || x.Content.ToLower().Contains(searchTerm));
        }

        var pagedResult = await query.ToPagedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return pagedResult.ToResponse();
    }
}
