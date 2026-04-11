namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public sealed class GetArticleByIdQueryHandler : IQueryHandler<GetArticleByIdQuery, GetArticleByIdResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetArticleByIdQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetArticleByIdResponse> Handle(GetArticleByIdQuery query, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.Articles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Article not found.");

        return entity.ToResponse();
    }
}

