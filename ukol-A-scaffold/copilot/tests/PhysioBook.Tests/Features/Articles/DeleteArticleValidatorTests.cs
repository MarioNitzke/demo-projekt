namespace PhysioBook.Tests.Features.Articles;

public sealed class DeleteArticleValidatorTests
{
    private readonly DeleteArticleValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Id_Is_Empty()
    {
        var command = new DeleteArticleCommand { Id = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Id_Is_Set()
    {
        var command = new DeleteArticleCommand { Id = Guid.NewGuid() };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}

