namespace PhysioBook.Features.TimeSlots.Commands.DeleteTimeSlot;

public class DeleteTimeSlotValidator : AbstractValidator<DeleteTimeSlotCommand>
{
    public DeleteTimeSlotValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
