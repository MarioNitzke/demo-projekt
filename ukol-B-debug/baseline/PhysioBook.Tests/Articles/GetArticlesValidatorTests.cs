using FluentAssertions;
using PhysioBook.Features.Articles.Queries.GetArticles;

namespace PhysioBook.Tests.Articles;

public class GetArticlesValidatorTests
{
    private readonly GetArticlesValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var query = new GetArticlesQuery { PageNumber = 1, PageSize = 10 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_PageNumber_Is_Zero()
    {
        var query = new GetArticlesQuery { PageNumber = 0, PageSize = 10 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Fail_When_PageSize_Exceeds_Limit()
    {
        var query = new GetArticlesQuery { PageNumber = 1, PageSize = 51 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
    }
}
