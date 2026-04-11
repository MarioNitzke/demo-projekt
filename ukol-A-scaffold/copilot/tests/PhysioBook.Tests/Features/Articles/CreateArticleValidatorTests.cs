namespace PhysioBook.Tests.Features.Articles;

public sealed class CreateArticleValidatorTests
{
    private readonly CreateArticleValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var command = new CreateArticleCommand { Title = string.Empty, Content = "ok" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new CreateArticleCommand { Title = "Title", Content = "Content" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}

