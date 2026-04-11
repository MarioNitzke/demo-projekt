namespace PhysioBook.Api.Features.Articles.Queries.GetArticleById;

public static class GetArticleByIdMappings
{
    public static GetArticleByIdQuery ToQuery(this GetArticleByIdRequest request)
        => new()
        {
            Id = request.Id
        };

    public static GetArticleByIdResponse ToResponse(this Article article)
        => new(
            article.Id,
            article.Title,
            article.Content,
            article.CreatedAtUtc,
            article.UpdatedAtUtc,
            article.CreatedByUserId);
}
