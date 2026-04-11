namespace PhysioBook.Api.Features.Articles.Commands.DeleteArticle;

public sealed record DeleteArticleRequest
{
    public Guid Id { get; init; }
}
