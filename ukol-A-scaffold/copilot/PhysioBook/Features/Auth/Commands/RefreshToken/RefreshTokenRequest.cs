namespace PhysioBook.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenRequest(string Email, string RefreshToken);

