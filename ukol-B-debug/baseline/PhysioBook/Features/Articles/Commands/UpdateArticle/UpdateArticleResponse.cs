namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public record UpdateArticleResponse(Guid Id, string Title, string Content, DateTime CreatedAt, DateTime? UpdatedAt, string? AuthorId);
