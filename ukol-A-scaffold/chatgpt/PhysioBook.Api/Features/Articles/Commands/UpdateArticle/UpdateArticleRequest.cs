namespace PhysioBook.Api.Features.Articles.Commands.UpdateArticle;

public sealed record UpdateArticleRequest
{
    public required string Title { get; init; }
    public required string Content { get; init; }
}
