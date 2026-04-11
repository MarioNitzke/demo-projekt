using FluentAssertions;
using PhysioBook.Api.Features.Articles.Queries.GetArticles;

namespace PhysioBook.Tests.Articles;

public sealed class GetArticlesValidatorTests
{
    private readonly GetArticlesValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenPageNumberIsZero()
    {
        var query = new GetArticlesQuery
        {
            PageNumber = 0,
            PageSize = 10
        };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(GetArticlesQuery.PageNumber));
    }

    [Fact]
    public void Validate_ShouldFail_WhenPageSizeIsTooLarge()
    {
        var query = new GetArticlesQuery
        {
            PageNumber = 1,
            PageSize = 101
        };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(GetArticlesQuery.PageSize));
    }

    [Fact]
    public void Validate_ShouldPass_WhenQueryIsValid()
    {
        var query = new GetArticlesQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }
}
