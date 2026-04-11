namespace PhysioBook.Features.TimeSlots.Commands.DeleteTimeSlot;

public static class DeleteTimeSlotMappings
{
    public static DeleteTimeSlotCommand ToCommand(this DeleteTimeSlotRequest request)
    {
        return new DeleteTimeSlotCommand
        {
            Id = request.Id
        };
    }
}
