using PhysioBook.Data.Shared;

namespace PhysioBook.Features.Articles.Queries.GetArticles;

public sealed class GetArticlesQuery : IQuery<GetArticlesResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

