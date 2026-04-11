namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public sealed class CreateArticleCommand : IQuery<CreateArticleResponse>
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? UserId { get; set; }
}

