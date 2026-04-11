namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public sealed class GetArticleByIdValidator : AbstractValidator<GetArticleByIdQuery>
{
    public GetArticleByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

