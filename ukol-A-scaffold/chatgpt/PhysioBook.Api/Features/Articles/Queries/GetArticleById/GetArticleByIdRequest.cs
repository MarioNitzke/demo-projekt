namespace PhysioBook.Api.Features.Articles.Queries.GetArticleById;

public sealed record GetArticleByIdRequest
{
    public Guid Id { get; init; }
}
