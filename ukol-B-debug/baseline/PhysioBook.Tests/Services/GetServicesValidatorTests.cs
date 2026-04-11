using FluentAssertions;
using PhysioBook.Features.Services.Queries.GetServices;

namespace PhysioBook.Tests.Services;

public class GetServicesValidatorTests
{
    private readonly GetServicesValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var query = new GetServicesQuery { PageNumber = 1, PageSize = 10 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_PageNumber_Is_Zero()
    {
        var query = new GetServicesQuery { PageNumber = 0, PageSize = 10 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public async Task Should_Fail_When_PageNumber_Is_Negative()
    {
        var query = new GetServicesQuery { PageNumber = -1, PageSize = 10 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public async Task Should_Fail_When_PageSize_Exceeds_50()
    {
        var query = new GetServicesQuery { PageNumber = 1, PageSize = 51 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
    }
}
