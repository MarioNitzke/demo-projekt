namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public sealed class UpdateArticleCommand : IQuery<UpdateArticleResponse>
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}

