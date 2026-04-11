using PhysioBook.Api.Middleware;

namespace PhysioBook.Api.Configurations;

public static class AppConfiguration
{
    public static void ConfigureApp(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("Frontend");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/health");
        app.RegisterEndpoints();
    }
}
