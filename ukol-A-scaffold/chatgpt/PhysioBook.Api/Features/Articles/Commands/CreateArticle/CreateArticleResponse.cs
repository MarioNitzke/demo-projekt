namespace PhysioBook.Api.Features.Articles.Commands.CreateArticle;

public sealed record CreateArticleResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? CreatedByUserId);
