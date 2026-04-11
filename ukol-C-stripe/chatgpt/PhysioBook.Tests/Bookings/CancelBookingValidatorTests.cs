using FluentAssertions;
using PhysioBook.Features.Bookings.Commands.CancelBooking;

namespace PhysioBook.Tests.Bookings;

public class CancelBookingValidatorTests
{
    private readonly CancelBookingValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var command = new CancelBookingCommand { Id = Guid.NewGuid() };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_Id_Is_Empty()
    {
        var command = new CancelBookingCommand { Id = Guid.Empty };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }
}
