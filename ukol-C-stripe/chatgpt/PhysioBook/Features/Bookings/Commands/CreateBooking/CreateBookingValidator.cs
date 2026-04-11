namespace PhysioBook.Features.Bookings.Commands.CreateBooking;

public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Booking must be in the future");
    }
}
