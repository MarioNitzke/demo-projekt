using FluentAssertions;
using PhysioBook.Features.TimeSlots.Commands.DeleteTimeSlot;

namespace PhysioBook.Tests.TimeSlots;

public class DeleteTimeSlotValidatorTests
{
    private readonly DeleteTimeSlotValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var command = new DeleteTimeSlotCommand { Id = Guid.NewGuid() };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_Id_Is_Empty()
    {
        var command = new DeleteTimeSlotCommand { Id = Guid.Empty };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }
}
