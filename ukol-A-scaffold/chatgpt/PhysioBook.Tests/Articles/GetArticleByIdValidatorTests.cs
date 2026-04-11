using FluentAssertions;
using PhysioBook.Api.Features.Articles.Queries.GetArticleById;

namespace PhysioBook.Tests.Articles;

public sealed class GetArticleByIdValidatorTests
{
    private readonly GetArticleByIdValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var query = new GetArticleByIdQuery
        {
            Id = Guid.Empty
        };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(GetArticleByIdQuery.Id));
    }

    [Fact]
    public void Validate_ShouldPass_WhenIdIsValid()
    {
        var query = new GetArticleByIdQuery
        {
            Id = Guid.NewGuid()
        };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }
}
