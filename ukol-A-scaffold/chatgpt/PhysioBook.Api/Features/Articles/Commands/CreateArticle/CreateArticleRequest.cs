namespace PhysioBook.Api.Features.Articles.Commands.CreateArticle;

public sealed record CreateArticleRequest
{
    public required string Title { get; init; }
    public required string Content { get; init; }
}
