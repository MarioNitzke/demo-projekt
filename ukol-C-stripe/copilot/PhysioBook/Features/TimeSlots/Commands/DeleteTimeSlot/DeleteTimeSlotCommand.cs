namespace PhysioBook.Features.TimeSlots.Commands.DeleteTimeSlot;

public class DeleteTimeSlotCommand : IQuery<DeleteTimeSlotResponse>
{
    public Guid Id { get; set; }
}
