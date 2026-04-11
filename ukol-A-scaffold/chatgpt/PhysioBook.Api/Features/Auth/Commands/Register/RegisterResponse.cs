namespace PhysioBook.Api.Features.Auth.Commands.Register;

public sealed record RegisterResponse(
    string UserId,
    string FullName,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    IReadOnlyCollection<string> Roles);
