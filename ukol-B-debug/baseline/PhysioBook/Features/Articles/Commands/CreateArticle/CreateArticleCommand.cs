namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public class CreateArticleCommand : IQuery<CreateArticleResponse>
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? AuthorId { get; set; }
}
