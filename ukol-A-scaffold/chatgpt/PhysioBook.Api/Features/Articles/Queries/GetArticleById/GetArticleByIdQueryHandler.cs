namespace PhysioBook.Api.Features.Articles.Queries.GetArticleById;

public sealed class GetArticleByIdQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    : IQueryHandler<GetArticleByIdQuery, GetArticleByIdResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory = contextFactory;

    public async Task<GetArticleByIdResponse> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.Articles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Article with id '{request.Id}' was not found.");

        return entity.ToResponse();
    }
}
