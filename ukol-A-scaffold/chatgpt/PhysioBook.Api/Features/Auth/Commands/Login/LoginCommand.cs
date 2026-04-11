namespace PhysioBook.Api.Features.Auth.Commands.Login;

public sealed class LoginCommand : IQuery<LoginResponse>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
