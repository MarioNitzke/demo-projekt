using FluentAssertions;
using PhysioBook.Api.Features.Articles.Commands.DeleteArticle;

namespace PhysioBook.Tests.Articles;

public sealed class DeleteArticleValidatorTests
{
    private readonly DeleteArticleValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var command = new DeleteArticleCommand
        {
            Id = Guid.Empty
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(DeleteArticleCommand.Id));
    }

    [Fact]
    public void Validate_ShouldPass_WhenIdIsValid()
    {
        var command = new DeleteArticleCommand
        {
            Id = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
