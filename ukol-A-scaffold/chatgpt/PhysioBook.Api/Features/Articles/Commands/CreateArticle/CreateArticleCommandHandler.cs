namespace PhysioBook.Api.Features.Articles.Commands.CreateArticle;

public sealed class CreateArticleCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    : IQueryHandler<CreateArticleCommand, CreateArticleResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory = contextFactory;

    public async Task<CreateArticleResponse> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var entity = request.ToEntity();
        context.Articles.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.ToResponse();
    }
}
