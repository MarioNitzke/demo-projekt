using PhysioBook.Data;

namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public class GetArticleByIdQueryHandler : IQueryHandler<GetArticleByIdQuery, GetArticleByIdResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetArticleByIdQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetArticleByIdResponse> Handle(GetArticleByIdQuery query, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var article = await context.Articles.FindAsync(new object[] { query.Id }, ct)
            ?? throw new KeyNotFoundException($"Article with Id '{query.Id}' was not found.");

        return article.ToResponse();
    }
}
