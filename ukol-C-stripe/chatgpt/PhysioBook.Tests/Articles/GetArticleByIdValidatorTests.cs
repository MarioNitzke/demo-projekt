using FluentAssertions;
using PhysioBook.Features.Articles.Queries.GetArticleById;

namespace PhysioBook.Tests.Articles;

public class GetArticleByIdValidatorTests
{
    private readonly GetArticleByIdValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var query = new GetArticleByIdQuery { Id = Guid.NewGuid() };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_Id_Is_Empty()
    {
        var query = new GetArticleByIdQuery { Id = Guid.Empty };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
    }
}
