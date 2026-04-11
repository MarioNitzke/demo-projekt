using PhysioBook.Data.Entities;

namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public static class UpdateArticleMappings
{
    public static UpdateArticleCommand ToCommand(this UpdateArticleRequest request, Guid id)
    {
        return new UpdateArticleCommand
        {
            Id = id,
            Title = request.Title,
            Content = request.Content
        };
    }

    public static void ApplyTo(this UpdateArticleCommand command, Article entity)
    {
        entity.Title = command.Title;
        entity.Content = command.Content;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static UpdateArticleResponse ToResponse(this Article article)
    {
        return new UpdateArticleResponse(
            article.Id,
            article.Title,
            article.Content,
            article.CreatedAt,
            article.UpdatedAt,
            article.AuthorId
        );
    }
}
