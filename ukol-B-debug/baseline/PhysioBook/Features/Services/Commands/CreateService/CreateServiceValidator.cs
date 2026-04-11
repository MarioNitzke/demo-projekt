namespace PhysioBook.Features.Services.Commands.CreateService;

public class CreateServiceValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceValidator()
    {
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
