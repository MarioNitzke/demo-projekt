using PhysioBook.Data.Entities;
using PhysioBook.Data.Shared;

namespace PhysioBook.Features.Articles.Queries.GetArticles;

public static class GetArticlesMappings
{
    public static GetArticlesQuery ToQuery(this GetArticlesRequest request)
    {
        return new GetArticlesQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public static GetArticlesResponse ToResponse(this PagedList<Article> pagedList)
    {
        return new GetArticlesResponse(
            pagedList.Items.Select(a => a.ToItemResponse()).ToList(),
            pagedList.TotalCount,
            pagedList.PageNumber,
            pagedList.PageSize,
            pagedList.TotalPages);
    }

    public static GetArticlesItemResponse ToItemResponse(this Article article)
    {
        return new GetArticlesItemResponse(
            article.Id,
            article.Title,
            article.Content,
            article.CreatedAt,
            article.UpdatedAt,
            article.AuthorId);
    }
}
