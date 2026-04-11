namespace PhysioBook.Api.Features.Articles.Commands.UpdateArticle;

public sealed record UpdateArticleResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? CreatedByUserId);
