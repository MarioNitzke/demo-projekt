using PhysioBook.Data;

namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public class CreateArticleCommandHandler : IQueryHandler<CreateArticleCommand, CreateArticleResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public CreateArticleCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<CreateArticleResponse> Handle(CreateArticleCommand command, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var article = command.ToEntity();

        context.Articles.Add(article);
        await context.SaveChangesAsync(ct);

        return article.ToResponse();
    }
}
