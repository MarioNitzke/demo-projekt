namespace PhysioBook.Api.Features.Articles.Queries.GetArticles;

public sealed record GetArticlesRequest
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public string? SearchTerm { get; init; }
}
