namespace PhysioBook.Features.Articles.Commands.DeleteArticle;

public sealed class DeleteArticleCommand : IQuery<DeleteArticleResponse>
{
    public Guid Id { get; set; }
}

