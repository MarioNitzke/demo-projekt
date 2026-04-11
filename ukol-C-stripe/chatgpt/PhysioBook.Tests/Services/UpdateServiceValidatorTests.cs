using FluentAssertions;
using PhysioBook.Features.Services.Commands.UpdateService;

namespace PhysioBook.Tests.Services;

public class UpdateServiceValidatorTests
{
    private readonly UpdateServiceValidator _validator = new();

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var command = new UpdateServiceCommand
        {
            Id = Guid.NewGuid(),
            Name = "Massage Therapy",
            Description = "A relaxing massage session",
            DurationMinutes = 60,
            Price = 500m
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_Id_Is_Empty()
    {
        var command = new UpdateServiceCommand
        {
            Id = Guid.Empty,
            Name = "Massage Therapy",
            Description = "A relaxing massage session",
            DurationMinutes = 60,
            Price = 500m
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Should_Fail_When_Name_Is_Empty()
    {
        var command = new UpdateServiceCommand
        {
            Id = Guid.NewGuid(),
            Name = "",
            Description = "A relaxing massage session",
            DurationMinutes = 60,
            Price = 500m
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Should_Fail_When_Name_Exceeds_MaxLength()
    {
        var command = new UpdateServiceCommand
        {
            Id = Guid.NewGuid(),
            Name = new string('A', 201),
            Description = "A relaxing massage session",
            DurationMinutes = 60,
            Price = 500m
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Should_Fail_When_Description_Is_Empty()
    {
        var command = new UpdateServiceCommand
        {
            Id = Guid.NewGuid(),
            Name = "Massage Therapy",
            Description = "",
            DurationMinutes = 60,
            Price = 500m
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task Should_Fail_When_DurationMinutes_Is_Zero()
    {
        var command = new UpdateServiceCommand
        {
            Id = Guid.NewGuid(),
            Name = "Massage Therapy",
            Description = "A relaxing massage session",
            DurationMinutes = 0,
            Price = 500m
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DurationMinutes");
    }

    [Fact]
    public async Task Should_Fail_When_Price_Is_Zero()
    {
        var command = new UpdateServiceCommand
        {
            Id = Guid.NewGuid(),
            Name = "Massage Therapy",
            Description = "A relaxing massage session",
            DurationMinutes = 60,
            Price = 0m
        };
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }
}
