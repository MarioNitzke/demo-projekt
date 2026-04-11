namespace PhysioBook.Api.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommand : IQuery<RefreshTokenResponse>
{
    public required string RefreshToken { get; init; }
}
