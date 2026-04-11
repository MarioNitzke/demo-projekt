namespace PhysioBook.Api.Features.Auth.Commands.Login;

public static class LoginMappings
{
    public static LoginCommand ToCommand(this LoginRequest request)
        => new()
        {
            Email = request.Email,
            Password = request.Password
        };

    public static LoginResponse ToResponse(this ApplicationUser user, TokenResult tokenResult)
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
