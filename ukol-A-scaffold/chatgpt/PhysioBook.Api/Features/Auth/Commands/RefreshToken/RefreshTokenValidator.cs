namespace PhysioBook.Api.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
