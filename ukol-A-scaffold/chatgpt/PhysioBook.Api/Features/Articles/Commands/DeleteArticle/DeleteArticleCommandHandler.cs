namespace PhysioBook.Api.Features.Articles.Commands.DeleteArticle;

public sealed class DeleteArticleCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    : IQueryHandler<DeleteArticleCommand, DeleteArticleResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory = contextFactory;

    public async Task<DeleteArticleResponse> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.Articles.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Article with id '{request.Id}' was not found.");

        context.Articles.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new DeleteArticleResponse(request.Id, "Article deleted successfully.");
    }
}
