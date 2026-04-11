using PhysioBook.Data.Entities;

namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public static class CreateArticleMappings
{
    public static CreateArticleCommand ToCommand(this CreateArticleRequest request)
    {
        return new CreateArticleCommand
        {
            Title = request.Title,
            Content = request.Content
        };
    }

    public static Article ToEntity(this CreateArticleCommand command)
    {
        return new Article
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Content = command.Content,
            CreatedAt = DateTime.UtcNow,
            AuthorId = command.AuthorId ?? string.Empty
        };
    }

    public static CreateArticleResponse ToResponse(this Article article)
    {
        return new CreateArticleResponse(
            article.Id,
            article.Title,
            article.Content,
            article.CreatedAt,
            article.AuthorId
        );
    }
}
