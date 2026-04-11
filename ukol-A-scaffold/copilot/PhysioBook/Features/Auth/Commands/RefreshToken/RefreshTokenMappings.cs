namespace PhysioBook.Features.Auth.Commands.RefreshToken;

public static class RefreshTokenMappings
{
    public static RefreshTokenCommand ToCommand(this RefreshTokenRequest request)
        => new()
        {
            Email = request.Email,
            RefreshToken = request.RefreshToken
        };
}

