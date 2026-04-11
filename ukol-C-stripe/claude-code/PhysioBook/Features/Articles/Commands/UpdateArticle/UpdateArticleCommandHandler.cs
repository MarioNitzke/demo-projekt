using PhysioBook.Data;

namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public class UpdateArticleCommandHandler : IQueryHandler<UpdateArticleCommand, UpdateArticleResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public UpdateArticleCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<UpdateArticleResponse> Handle(UpdateArticleCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var article = await context.Articles.FindAsync(new object[] { command.Id }, ct)
            ?? throw new KeyNotFoundException($"Article with Id '{command.Id}' was not found.");

        command.ApplyTo(article);
        await context.SaveChangesAsync(ct);

        return article.ToResponse();
    }
}
