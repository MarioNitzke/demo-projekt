using FluentAssertions;
using PhysioBook.Features.Articles.Commands.CreateArticle;

namespace PhysioBook.Tests.Articles;

public class CreateArticleValidatorTests
{
    private readonly CreateArticleValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var command = new CreateArticleCommand { Title = "Test Title", Content = "Test Content" };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_Title_Is_Empty()
    {
        var command = new CreateArticleCommand { Title = "", Content = "Test Content" };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Should_Fail_When_Title_Exceeds_MaxLength()
    {
        var command = new CreateArticleCommand { Title = new string('A', 201), Content = "Test Content" };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Should_Fail_When_Content_Is_Empty()
    {
        var command = new CreateArticleCommand { Title = "Test Title", Content = "" };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Content");
    }
}
