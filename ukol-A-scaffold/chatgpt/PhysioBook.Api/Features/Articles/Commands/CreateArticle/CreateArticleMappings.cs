namespace PhysioBook.Api.Features.Articles.Commands.CreateArticle;

public static class CreateArticleMappings
{
    public static CreateArticleCommand ToCommand(this CreateArticleRequest request, string? userId)
        => new()
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId
        };

    public static Article ToEntity(this CreateArticleCommand command)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = command.Title.Trim(),
            Content = command.Content.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = command.UserId
        };

    public static CreateArticleResponse ToResponse(this Article article)
        => new(
            article.Id,
            article.Title,
            article.Content,
            article.CreatedAtUtc,
            article.UpdatedAtUtc,
            article.CreatedByUserId);
}
