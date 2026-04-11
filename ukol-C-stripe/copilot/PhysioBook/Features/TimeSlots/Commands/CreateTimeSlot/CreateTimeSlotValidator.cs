namespace PhysioBook.Features.TimeSlots.Commands.CreateTimeSlot;

public class CreateTimeSlotValidator : AbstractValidator<CreateTimeSlotCommand>
{
    public CreateTimeSlotValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween(0, 6);

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");
    }
}
