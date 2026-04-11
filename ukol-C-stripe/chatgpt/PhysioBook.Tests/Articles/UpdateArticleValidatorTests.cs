using FluentAssertions;
using PhysioBook.Features.Articles.Commands.UpdateArticle;

namespace PhysioBook.Tests.Articles;

public class UpdateArticleValidatorTests
{
    private readonly UpdateArticleValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var command = new UpdateArticleCommand { Id = Guid.NewGuid(), Title = "Updated Title", Content = "Updated Content" };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_Id_Is_Empty()
    {
        var command = new UpdateArticleCommand { Id = Guid.Empty, Title = "Title", Content = "Content" };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Should_Fail_When_Title_Is_Empty()
    {
        var command = new UpdateArticleCommand { Id = Guid.NewGuid(), Title = "", Content = "Content" };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Fail_When_Title_Exceeds_MaxLength()
    {
        var command = new UpdateArticleCommand { Id = Guid.NewGuid(), Title = new string('A', 201), Content = "Content" };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Fail_When_Content_Is_Empty()
    {
        var command = new UpdateArticleCommand { Id = Guid.NewGuid(), Title = "Title", Content = "" };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
    }
}
