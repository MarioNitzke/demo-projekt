namespace PhysioBook.Features.TimeSlots.Commands.CreateTimeSlot;

public record CreateTimeSlotResponse(Guid Id, int DayOfWeek, string StartTime, string EndTime, bool IsAvailable);
