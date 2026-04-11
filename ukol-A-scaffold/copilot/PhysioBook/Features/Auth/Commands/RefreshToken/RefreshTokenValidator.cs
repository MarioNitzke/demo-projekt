namespace PhysioBook.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

