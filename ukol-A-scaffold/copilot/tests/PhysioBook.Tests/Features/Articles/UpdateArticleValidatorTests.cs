namespace PhysioBook.Tests.Features.Articles;

public sealed class UpdateArticleValidatorTests
{
    private readonly UpdateArticleValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Id_Is_Empty()
    {
        var command = new UpdateArticleCommand { Id = Guid.Empty, Title = "T", Content = "C" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new UpdateArticleCommand { Id = Guid.NewGuid(), Title = "T", Content = "C" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}

