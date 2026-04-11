using PhysioBook.Api.Data.Shared;

namespace PhysioBook.Api.Features.Articles.Queries.GetArticles;

public static class GetArticlesMappings
{
    public static GetArticlesQuery ToQuery(this GetArticlesRequest request)
        => new()
        {
            PageNumber = request.PageNumber ?? 1,
            PageSize = request.PageSize ?? 10,
            SearchTerm = request.SearchTerm
        };

    public static GetArticlesResponse ToResponse(this PagedList<Article> page)
        => new(
            page.Items.Select(article => article.ToListItemResponse()).ToArray(),
            page.PageNumber,
            page.PageSize,
            page.TotalCount,
            page.TotalPages);

    public static ArticleListItemResponse ToListItemResponse(this Article article)
        => new(
            article.Id,
            article.Title,
            article.Content.Length <= 120 ? article.Content : $"{article.Content[..120]}...",
            article.CreatedAtUtc,
            article.UpdatedAtUtc,
            article.CreatedByUserId);
}
