namespace PhysioBook.Api.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}
