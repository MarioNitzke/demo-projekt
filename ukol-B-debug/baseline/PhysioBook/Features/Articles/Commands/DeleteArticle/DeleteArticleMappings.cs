namespace PhysioBook.Features.Articles.Commands.DeleteArticle;

public static class DeleteArticleMappings
{
    public static DeleteArticleCommand ToCommand(this DeleteArticleRequest request)
    {
        return new DeleteArticleCommand
        {
            Id = request.Id
        };
    }
}
