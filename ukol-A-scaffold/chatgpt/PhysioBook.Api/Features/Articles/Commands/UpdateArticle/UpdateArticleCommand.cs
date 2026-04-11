namespace PhysioBook.Api.Features.Articles.Commands.UpdateArticle;

public sealed class UpdateArticleCommand : IQuery<UpdateArticleResponse>
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public string? UserId { get; init; }
}
