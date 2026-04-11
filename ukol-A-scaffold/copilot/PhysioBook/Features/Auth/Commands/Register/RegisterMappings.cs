namespace PhysioBook.Features.Auth.Commands.Register;

public static class RegisterMappings
{
    public static RegisterCommand ToCommand(this RegisterRequest request)
        => new()
        {
            Email = request.Email,
            Password = request.Password
        };
}

