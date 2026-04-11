namespace PhysioBook.Features.Articles.Queries.GetArticles;

public sealed record GetArticlesResponse(IReadOnlyCollection<GetArticlesResponseItem> Items, int PageNumber, int PageSize, int TotalCount);

public sealed record GetArticlesResponseItem(Guid Id, string Title, string Content, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc, string? UserId);

