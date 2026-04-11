using PhysioBook.Data;

namespace PhysioBook.Features.Articles.Commands.DeleteArticle;

public class DeleteArticleCommandHandler : IQueryHandler<DeleteArticleCommand, DeleteArticleResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public DeleteArticleCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<DeleteArticleResponse> Handle(DeleteArticleCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var article = await context.Articles.FindAsync(new object[] { command.Id }, ct)
            ?? throw new KeyNotFoundException($"Article with Id '{command.Id}' was not found.");

        context.Articles.Remove(article);
        await context.SaveChangesAsync(ct);

        return new DeleteArticleResponse(true);
    }
}
