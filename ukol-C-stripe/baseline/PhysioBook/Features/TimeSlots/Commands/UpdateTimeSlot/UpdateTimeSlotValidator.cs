namespace PhysioBook.Features.TimeSlots.Commands.UpdateTimeSlot;

public class UpdateTimeSlotValidator : AbstractValidator<UpdateTimeSlotCommand>
{
    public UpdateTimeSlotValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween(0, 6);

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");
    }
}
