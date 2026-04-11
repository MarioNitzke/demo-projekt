namespace PhysioBook.Api.Auth;

public sealed record TokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    IReadOnlyCollection<string> Roles);
