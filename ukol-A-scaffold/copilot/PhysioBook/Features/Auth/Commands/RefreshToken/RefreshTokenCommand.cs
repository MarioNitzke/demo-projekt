namespace PhysioBook.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommand : IQuery<RefreshTokenResponse>
{
    public required string Email { get; set; }
    public required string RefreshToken { get; set; }
}

