using FluentAssertions;
using PhysioBook.Features.Bookings.Commands.CreateCheckoutSession;

namespace PhysioBook.Tests.Bookings;

public class CreateCheckoutSessionValidatorTests
{
    private readonly CreateCheckoutSessionValidator _validator = new();

    [Fact]
    public void Should_Fail_When_BookingId_Is_Empty()
    {
        var command = new CreateCheckoutSessionCommand
        {
            BookingId = Guid.Empty,
            UserId = "user-1"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCheckoutSessionCommand.BookingId));
    }

    [Fact]
    public void Should_Fail_When_UserId_Is_Missing()
    {
        var command = new CreateCheckoutSessionCommand
        {
            BookingId = Guid.NewGuid(),
            UserId = string.Empty
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCheckoutSessionCommand.UserId));
    }

    [Fact]
    public void Should_Pass_When_Request_Is_Valid()
    {
        var command = new CreateCheckoutSessionCommand
        {
            BookingId = Guid.NewGuid(),
            UserId = "user-1"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}

