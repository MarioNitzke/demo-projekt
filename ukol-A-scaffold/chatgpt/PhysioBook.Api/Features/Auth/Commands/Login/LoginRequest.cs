namespace PhysioBook.Api.Features.Auth.Commands.Login;

public sealed record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
