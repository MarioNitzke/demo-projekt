using PhysioBook.Data.Shared;

namespace PhysioBook.Features.Articles.Queries.GetArticles;

public static class GetArticlesMappings
{
    public static GetArticlesQuery ToQuery(this GetArticlesRequest request)
        => new()
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

    public static GetArticlesResponse ToResponse(this PagedList<Article> paged)
        => new(
            paged.Items.Select(x => new GetArticlesResponseItem(x.Id, x.Title, x.Content, x.CreatedAtUtc, x.UpdatedAtUtc, x.UserId)).ToArray(),
            paged.PageNumber,
            paged.PageSize,
            paged.TotalCount);
}

