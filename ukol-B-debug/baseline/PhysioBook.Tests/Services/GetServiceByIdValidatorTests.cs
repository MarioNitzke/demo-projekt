using FluentAssertions;
using PhysioBook.Features.Services.Queries.GetServiceById;

namespace PhysioBook.Tests.Services;

public class GetServiceByIdValidatorTests
{
    private readonly GetServiceByIdValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var query = new GetServiceByIdQuery { Id = Guid.NewGuid() };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_Id_Is_Empty()
    {
        var query = new GetServiceByIdQuery { Id = Guid.Empty };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }
}
