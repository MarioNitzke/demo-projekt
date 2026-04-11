namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public sealed record GetArticleByIdResponse(Guid Id, string Title, string Content, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc, string? UserId);

