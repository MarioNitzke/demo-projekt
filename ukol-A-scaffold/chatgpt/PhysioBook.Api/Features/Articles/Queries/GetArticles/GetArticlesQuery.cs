using PhysioBook.Api.Data.Shared;

namespace PhysioBook.Api.Features.Articles.Queries.GetArticles;

public sealed class GetArticlesQuery : IQuery<GetArticlesResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
}
