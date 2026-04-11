using FluentAssertions;
using PhysioBook.Api.Features.Articles.Commands.CreateArticle;

namespace PhysioBook.Tests.Articles;

public sealed class CreateArticleValidatorTests
{
    private readonly CreateArticleValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenTitleIsEmpty()
    {
        var command = new CreateArticleCommand
        {
            Title = string.Empty,
            Content = "Valid article content."
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateArticleCommand.Title));
    }

    [Fact]
    public void Validate_ShouldFail_WhenContentIsTooShort()
    {
        var command = new CreateArticleCommand
        {
            Title = "Valid title",
            Content = "short"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateArticleCommand.Content));
    }

    [Fact]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new CreateArticleCommand
        {
            Title = "Article title",
            Content = "This is a valid article content body with enough characters."
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
