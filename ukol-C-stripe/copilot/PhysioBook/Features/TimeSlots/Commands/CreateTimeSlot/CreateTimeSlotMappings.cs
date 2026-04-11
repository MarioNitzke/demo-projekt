using PhysioBook.Data.Entities;

namespace PhysioBook.Features.TimeSlots.Commands.CreateTimeSlot;

public static class CreateTimeSlotMappings
{
    public static CreateTimeSlotCommand ToCommand(this CreateTimeSlotRequest request) => new()
    {
        DayOfWeek = request.DayOfWeek,
        StartTime = TimeOnly.Parse(request.StartTime),
        EndTime = TimeOnly.Parse(request.EndTime),
        IsAvailable = request.IsAvailable
    };

    public static TimeSlot ToEntity(this CreateTimeSlotCommand command)
    {
        return new TimeSlot
        {
            Id = Guid.NewGuid(),
            DayOfWeek = (DayOfWeek)command.DayOfWeek,
            StartTime = command.StartTime,
            EndTime = command.EndTime,
            IsAvailable = command.IsAvailable
        };
    }

    public static CreateTimeSlotResponse ToResponse(this TimeSlot entity)
    {
        return new CreateTimeSlotResponse(
            entity.Id,
            (int)entity.DayOfWeek,
            entity.StartTime.ToString("HH:mm"),
            entity.EndTime.ToString("HH:mm"),
            entity.IsAvailable
        );
    }
}
