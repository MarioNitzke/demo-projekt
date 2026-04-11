namespace PhysioBook.Features.Auth.Commands.Login;

public sealed class LoginCommand : IQuery<LoginResponse>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

