namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public record CreateArticleResponse(Guid Id, string Title, string Content, DateTime CreatedAt, string? AuthorId);
