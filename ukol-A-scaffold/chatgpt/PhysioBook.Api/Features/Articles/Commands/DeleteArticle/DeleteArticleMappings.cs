namespace PhysioBook.Api.Features.Articles.Commands.DeleteArticle;

public static class DeleteArticleMappings
{
    public static DeleteArticleCommand ToCommand(this DeleteArticleRequest request)
        => new()
        {
            Id = request.Id
        };
}
