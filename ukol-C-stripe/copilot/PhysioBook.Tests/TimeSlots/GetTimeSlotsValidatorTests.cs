using FluentAssertions;
using PhysioBook.Features.TimeSlots.Queries.GetTimeSlots;

namespace PhysioBook.Tests.TimeSlots;

public class GetTimeSlotsValidatorTests
{
    private readonly GetTimeSlotsValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid_Without_DayOfWeek()
    {
        var query = new GetTimeSlotsQuery { DayOfWeek = null };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Pass_When_Valid_With_DayOfWeek()
    {
        var query = new GetTimeSlotsQuery { DayOfWeek = 3 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_DayOfWeek_Is_Negative()
    {
        var query = new GetTimeSlotsQuery { DayOfWeek = -1 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DayOfWeek");
    }

    [Fact]
    public async Task Should_Fail_When_DayOfWeek_Is_Greater_Than_6()
    {
        var query = new GetTimeSlotsQuery { DayOfWeek = 7 };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DayOfWeek");
    }
}
