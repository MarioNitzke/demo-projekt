namespace PhysioBook.Features.Services.Commands.UpdateService;

public class UpdateServiceValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0);

        RuleFor(x => x.Price)
            .GreaterThan(0);
    }
}
