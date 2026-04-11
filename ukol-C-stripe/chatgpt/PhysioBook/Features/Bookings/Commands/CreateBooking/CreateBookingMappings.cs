using PhysioBook.Data.Entities;
using PhysioBook.Data.Enums;

namespace PhysioBook.Features.Bookings.Commands.CreateBooking;

public static class CreateBookingMappings
{
    public static CreateBookingCommand ToCommand(this CreateBookingRequest request)
    {
        return new CreateBookingCommand
        {
            ServiceId = request.ServiceId,
            StartTime = request.StartTime,
            Notes = request.Notes
        };
    }

    public static Booking ToEntity(this CreateBookingCommand command, DateTime endTime)
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            ClientId = command.ClientId ?? string.Empty,
            ServiceId = command.ServiceId,
            StartTime = command.StartTime,
            EndTime = endTime,
            Status = BookingStatus.Pending,
            Notes = command.Notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static CreateBookingResponse ToResponse(this Booking booking)
    {
        return new CreateBookingResponse(
            booking.Id,
            booking.ClientId,
            booking.ServiceId,
            booking.StartTime,
            booking.EndTime,
            booking.Status.ToString(),
            booking.PaymentStatus.ToString(),
            booking.Notes,
            booking.CreatedAt);
    }
}
