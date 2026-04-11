namespace PhysioBook.Tests.Features.Articles;

public sealed class GetArticleByIdValidatorTests
{
    private readonly GetArticleByIdValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Id_Is_Empty()
    {
        var query = new GetArticleByIdQuery { Id = Guid.Empty };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Id_Is_Valid()
    {
        var query = new GetArticleByIdQuery { Id = Guid.NewGuid() };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }
}

