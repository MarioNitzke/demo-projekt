using PhysioBook.Data.Entities;

namespace PhysioBook.Features.TimeSlots.Commands.UpdateTimeSlot;

public static class UpdateTimeSlotMappings
{
    public static UpdateTimeSlotCommand ToCommand(this UpdateTimeSlotRequest request, Guid id)
    {
        return new UpdateTimeSlotCommand
        {
            Id = id,
            DayOfWeek = request.DayOfWeek,
            StartTime = TimeOnly.Parse(request.StartTime),
            EndTime = TimeOnly.Parse(request.EndTime),
            IsAvailable = request.IsAvailable
        };
    }

    public static void ApplyTo(this UpdateTimeSlotCommand command, TimeSlot entity)
    {
        entity.DayOfWeek = (DayOfWeek)command.DayOfWeek;
        entity.StartTime = command.StartTime;
        entity.EndTime = command.EndTime;
        entity.IsAvailable = command.IsAvailable;
    }

    public static UpdateTimeSlotResponse ToResponse(this TimeSlot entity)
    {
        return new UpdateTimeSlotResponse(
            entity.Id,
            (int)entity.DayOfWeek,
            entity.StartTime.ToString("HH:mm"),
            entity.EndTime.ToString("HH:mm"),
            entity.IsAvailable
        );
    }
}
