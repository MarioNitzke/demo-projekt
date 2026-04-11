namespace PhysioBook.Api.Features.Auth.Commands.Login;

public sealed record LoginResponse(
    string UserId,
    string FullName,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    IReadOnlyCollection<string> Roles);
