namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public sealed record CreateArticleResponse(Guid Id, string Title, string Content, DateTime CreatedAtUtc, string? UserId);

