namespace PhysioBook.Api.Features.Auth.Commands.Register;

public static class RegisterMappings
{
    public static RegisterCommand ToCommand(this RegisterRequest request)
        => new()
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = request.Password
        };

    public static ApplicationUser ToEntity(this RegisterCommand command)
        => new()
        {
            Id = Guid.NewGuid().ToString(),
            FullName = command.FullName.Trim(),
            Email = command.Email.Trim().ToLowerInvariant(),
            UserName = command.Email.Trim().ToLowerInvariant()
        };

    public static RegisterResponse ToResponse(this ApplicationUser user, TokenResult tokenResult)
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
