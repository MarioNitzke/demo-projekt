namespace PhysioBook.Api.Features.Articles.Commands.CreateArticle;

public sealed class CreateArticleCommand : IQuery<CreateArticleResponse>
{
    public required string Title { get; init; }
    public required string Content { get; init; }
    public string? UserId { get; init; }
}
