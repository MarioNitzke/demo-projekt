using FluentAssertions;
using PhysioBook.Features.Bookings.Queries.GetAvailability;

namespace PhysioBook.Tests.Bookings;

public class GetAvailabilityValidatorTests
{
    private readonly GetAvailabilityValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var query = new GetAvailabilityQuery
        {
            ServiceId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_ServiceId_Is_Empty()
    {
        var query = new GetAvailabilityQuery
        {
            ServiceId = Guid.Empty,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ServiceId");
    }
}
