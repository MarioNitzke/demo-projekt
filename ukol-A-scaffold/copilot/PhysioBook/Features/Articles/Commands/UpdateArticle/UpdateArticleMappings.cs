namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public static class UpdateArticleMappings
{
    public static UpdateArticleCommand ToCommand(this UpdateArticleRequest request, Guid id)
        => new()
        {
            Id = id,
            Title = request.Title,
            Content = request.Content
        };

    public static UpdateArticleResponse ToResponse(this Article article)
        => new(article.Id, article.Title, article.Content, article.CreatedAtUtc, article.UpdatedAtUtc, article.UserId);
}

