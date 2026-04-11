using PhysioBook.Data.Entities;

namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public static class GetArticleByIdMappings
{
    public static GetArticleByIdQuery ToQuery(this GetArticleByIdRequest request)
    {
        return new GetArticleByIdQuery
        {
            Id = request.Id
        };
    }

    public static GetArticleByIdResponse ToResponse(this Article article)
    {
        return new GetArticleByIdResponse(
            article.Id,
            article.Title,
            article.Content,
            article.CreatedAt,
            article.UpdatedAt,
            article.AuthorId);
    }
}
