namespace PhysioBook.Features.Articles.Commands.DeleteArticle;

public static class DeleteArticleMappings
{
    public static DeleteArticleCommand ToCommand(this Guid id)
        => new()
        {
            Id = id
        };
}

