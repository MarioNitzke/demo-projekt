namespace PhysioBook.Features.Articles.Commands.CreateArticle;

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
            Title = command.Title,
            Content = command.Content,
            UserId = command.UserId,
            CreatedAtUtc = DateTime.UtcNow
        };

    public static CreateArticleResponse ToResponse(this Article article)
        => new(article.Id, article.Title, article.Content, article.CreatedAtUtc, article.UserId);
}

