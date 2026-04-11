namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public sealed class CreateArticleCommandHandler : IQueryHandler<CreateArticleCommand, CreateArticleResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public CreateArticleCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<CreateArticleResponse> Handle(CreateArticleCommand query, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var entity = query.ToEntity();

        await context.Articles.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.ToResponse();
    }
}

