namespace PhysioBook.Features.Bookings.Commands.CreateCheckoutSession;

public class CreateCheckoutSessionValidator : AbstractValidator<CreateCheckoutSessionCommand>
{
    public CreateCheckoutSessionValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("BookingId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User must be authenticated");
    }
}

