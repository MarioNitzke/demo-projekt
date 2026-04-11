namespace PhysioBook.Features.Articles.Queries.GetArticles;

public record GetArticlesResponse(
    List<GetArticlesItemResponse> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

public record GetArticlesItemResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? AuthorId);
