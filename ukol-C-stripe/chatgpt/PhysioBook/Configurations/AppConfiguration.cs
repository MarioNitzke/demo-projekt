using PhysioBook.Features.Payments;
using PhysioBook.Middleware;

namespace PhysioBook.Configurations;

public static class AppConfiguration
{
    public static void ConfigureApp(this WebApplication app)
    {
        app.UseExceptionHandler(_ => { });

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<CorrelationIdMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PhysioBook API v1");
            });
        }

        app.MapEndpoints();
        app.MapStripeWebhookEndpoint();
    }
}
