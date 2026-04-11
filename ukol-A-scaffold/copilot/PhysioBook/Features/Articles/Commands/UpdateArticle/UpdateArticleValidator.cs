namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public sealed class UpdateArticleValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(5000);
    }
}

