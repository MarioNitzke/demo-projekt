namespace PhysioBook.Api.Features.Articles.Commands.UpdateArticle;

public sealed class UpdateArticleCommandHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    : IQueryHandler<UpdateArticleCommand, UpdateArticleResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory = contextFactory;

    public async Task<UpdateArticleResponse> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.Articles.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Article with id '{request.Id}' was not found.");

        entity.Title = request.Title.Trim();
        entity.Content = request.Content.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return entity.ToResponse();
    }
}
