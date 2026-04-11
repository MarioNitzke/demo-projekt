using PhysioBook.Features.Auth.Commands.Login;
using PhysioBook.Features.Auth.Commands.RefreshToken;
using PhysioBook.Features.Auth.Commands.Register;

namespace PhysioBook.Features.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRegisterEndpoint();
        app.MapLoginEndpoint();
        app.MapRefreshTokenEndpoint();

        return app;
    }
}

