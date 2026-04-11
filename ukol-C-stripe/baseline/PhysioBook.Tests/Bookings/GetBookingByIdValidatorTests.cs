using FluentAssertions;
using PhysioBook.Features.Bookings.Queries.GetBookingById;

namespace PhysioBook.Tests.Bookings;

public class GetBookingByIdValidatorTests
{
    private readonly GetBookingByIdValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var query = new GetBookingByIdQuery { Id = Guid.NewGuid() };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_Id_Is_Empty()
    {
        var query = new GetBookingByIdQuery { Id = Guid.Empty };
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }
}
