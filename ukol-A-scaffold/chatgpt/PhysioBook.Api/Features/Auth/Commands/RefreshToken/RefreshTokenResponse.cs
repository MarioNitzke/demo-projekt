namespace PhysioBook.Api.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenResponse(
    string UserId,
    string FullName,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    IReadOnlyCollection<string> Roles);
