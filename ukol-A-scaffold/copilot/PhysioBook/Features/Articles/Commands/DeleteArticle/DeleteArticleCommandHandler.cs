namespace PhysioBook.Features.Articles.Commands.DeleteArticle;

public sealed class DeleteArticleCommandHandler : IQueryHandler<DeleteArticleCommand, DeleteArticleResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public DeleteArticleCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DeleteArticleResponse> Handle(DeleteArticleCommand query, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await context.Articles.FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
                     ?? throw new KeyNotFoundException("Article not found.");

        context.Articles.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new DeleteArticleResponse(query.Id, true);
    }
}

