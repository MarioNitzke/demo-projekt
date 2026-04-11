namespace PhysioBook.Features.Articles.Commands.DeleteArticle;

public sealed class DeleteArticleValidator : AbstractValidator<DeleteArticleCommand>
{
    public DeleteArticleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

