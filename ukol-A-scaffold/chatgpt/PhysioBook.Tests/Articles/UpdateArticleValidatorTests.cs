using FluentAssertions;
using PhysioBook.Api.Features.Articles.Commands.UpdateArticle;

namespace PhysioBook.Tests.Articles;

public sealed class UpdateArticleValidatorTests
{
    private readonly UpdateArticleValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var command = new UpdateArticleCommand
        {
            Id = Guid.Empty,
            Title = "Valid title",
            Content = "This is a valid article content body with enough characters."
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(UpdateArticleCommand.Id));
    }

    [Fact]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var command = new UpdateArticleCommand
        {
            Id = Guid.NewGuid(),
            Title = "Valid title",
            Content = "This is a valid article content body with enough characters."
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
