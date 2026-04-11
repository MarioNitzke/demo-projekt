namespace PhysioBook.Api.Features.Articles.Queries.GetArticleById;

public sealed record GetArticleByIdResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? CreatedByUserId);
