namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public sealed class UpdateArticleCommandHandler : IQueryHandler<UpdateArticleCommand, UpdateArticleResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public UpdateArticleCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<UpdateArticleResponse> Handle(UpdateArticleCommand query, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await context.Articles.FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
                     ?? throw new KeyNotFoundException("Article not found.");

        entity.Title = query.Title;
        entity.Content = query.Content;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }
}

