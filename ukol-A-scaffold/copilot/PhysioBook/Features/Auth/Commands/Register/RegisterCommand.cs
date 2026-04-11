namespace PhysioBook.Features.Auth.Commands.Register;

public sealed class RegisterCommand : IQuery<RegisterResponse>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string Role { get; set; } = AppRoles.Admin;
}

