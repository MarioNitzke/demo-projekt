namespace PhysioBook.Features.Auth.Commands.Login;

public static class LoginMappings
{
    public static LoginCommand ToCommand(this LoginRequest request)
        => new()
        {
            Email = request.Email,
            Password = request.Password
        };
}

