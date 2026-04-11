using FluentAssertions;
using PhysioBook.Features.TimeSlots.Commands.CreateTimeSlot;

namespace PhysioBook.Tests.TimeSlots;

public class CreateTimeSlotValidatorTests
{
    private readonly CreateTimeSlotValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var command = new CreateTimeSlotCommand
        {
            DayOfWeek = 1,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 0),
            IsAvailable = true
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_DayOfWeek_Is_Negative()
    {
        var command = new CreateTimeSlotCommand
        {
            DayOfWeek = -1,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 0),
            IsAvailable = true
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DayOfWeek");
    }

    [Fact]
    public async Task Should_Fail_When_DayOfWeek_Is_Greater_Than_6()
    {
        var command = new CreateTimeSlotCommand
        {
            DayOfWeek = 7,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 0),
            IsAvailable = true
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DayOfWeek");
    }

    [Fact]
    public async Task Should_Fail_When_StartTime_Equals_EndTime()
    {
        var command = new CreateTimeSlotCommand
        {
            DayOfWeek = 1,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(10, 0),
            IsAvailable = true
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StartTime");
    }

    [Fact]
    public async Task Should_Fail_When_StartTime_Is_After_EndTime()
    {
        var command = new CreateTimeSlotCommand
        {
            DayOfWeek = 1,
            StartTime = new TimeOnly(14, 0),
            EndTime = new TimeOnly(10, 0),
            IsAvailable = true
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StartTime");
    }
}
