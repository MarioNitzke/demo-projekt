namespace PhysioBook.Api.Features.Auth.Commands.RefreshToken;

public static class RefreshTokenMappings
{
    public static RefreshTokenCommand ToCommand(this RefreshTokenRequest request)
        => new()
        {
            RefreshToken = request.RefreshToken
        };

    public static RefreshTokenResponse ToResponse(this ApplicationUser user, TokenResult tokenResult)
        => new(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            tokenResult.AccessToken,
            tokenResult.RefreshToken,
            tokenResult.ExpiresAtUtc,
            tokenResult.RefreshTokenExpiresAtUtc,
            tokenResult.Roles);
}
