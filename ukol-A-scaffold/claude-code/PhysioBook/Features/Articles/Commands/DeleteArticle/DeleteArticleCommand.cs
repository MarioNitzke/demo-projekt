namespace PhysioBook.Features.Articles.Commands.DeleteArticle;

public class DeleteArticleCommand : IQuery<DeleteArticleResponse>
{
    public Guid Id { get; set; }
}
