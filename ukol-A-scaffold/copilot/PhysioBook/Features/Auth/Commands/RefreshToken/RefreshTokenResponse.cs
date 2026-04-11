namespace PhysioBook.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);

