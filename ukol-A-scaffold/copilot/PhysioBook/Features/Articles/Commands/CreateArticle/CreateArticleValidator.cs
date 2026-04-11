namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public sealed class CreateArticleValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(5000);
    }
}

