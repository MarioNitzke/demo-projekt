namespace PhysioBook.Api.Features.Articles.Queries.GetArticles;

public sealed record GetArticlesResponse(
    IReadOnlyCollection<ArticleListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed record ArticleListItemResponse(
    Guid Id,
    string Title,
    string Preview,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? CreatedByUserId);
