using FluentAssertions;
using PhysioBook.Features.Bookings.Commands.CreateBooking;

namespace PhysioBook.Tests.Bookings;

public class CreateBookingValidatorTests
{
    private readonly CreateBookingValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var command = new CreateBookingCommand
        {
            ServiceId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1)
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_ServiceId_Is_Empty()
    {
        var command = new CreateBookingCommand
        {
            ServiceId = Guid.Empty,
            StartTime = DateTime.UtcNow.AddDays(1)
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ServiceId");
    }

    [Fact]
    public async Task Should_Fail_When_StartTime_Is_In_Past()
    {
        var command = new CreateBookingCommand
        {
            ServiceId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddHours(-1)
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StartTime");
    }
}
