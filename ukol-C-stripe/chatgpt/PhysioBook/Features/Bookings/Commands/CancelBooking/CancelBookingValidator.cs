namespace PhysioBook.Features.Bookings.Commands.CancelBooking;

public class CancelBookingValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
