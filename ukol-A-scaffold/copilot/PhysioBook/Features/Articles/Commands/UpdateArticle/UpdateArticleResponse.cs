namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public sealed record UpdateArticleResponse(Guid Id, string Title, string Content, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc, string? UserId);

