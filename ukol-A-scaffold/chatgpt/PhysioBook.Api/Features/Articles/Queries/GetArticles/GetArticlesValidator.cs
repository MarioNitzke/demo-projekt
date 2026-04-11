namespace PhysioBook.Api.Features.Articles.Queries.GetArticles;

public sealed class GetArticlesValidator : AbstractValidator<GetArticlesQuery>
{
    public GetArticlesValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
