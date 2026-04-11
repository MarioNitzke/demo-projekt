namespace PhysioBook.Features.Auth.Commands.Login;

public sealed record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);

