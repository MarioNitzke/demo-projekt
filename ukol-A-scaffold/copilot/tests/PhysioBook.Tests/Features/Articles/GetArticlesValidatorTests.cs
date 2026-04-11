namespace PhysioBook.Tests.Features.Articles;

public sealed class GetArticlesValidatorTests
{
    private readonly GetArticlesValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Page_Number_Is_Zero()
    {
        var query = new GetArticlesQuery { PageNumber = 0, PageSize = 10 };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Pagination_Is_Valid()
    {
        var query = new GetArticlesQuery { PageNumber = 1, PageSize = 10 };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }
}

