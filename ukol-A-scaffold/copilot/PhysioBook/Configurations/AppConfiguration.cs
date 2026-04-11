namespace PhysioBook.Configurations;

public static class AppConfiguration
{
    public static void ConfigureApp(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("Frontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.RegisterEndpoints();
    }
}

