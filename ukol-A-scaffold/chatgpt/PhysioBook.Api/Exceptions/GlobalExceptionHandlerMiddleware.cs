using System.Text.Json;

namespace PhysioBook.Api.Exceptions;

public sealed class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Title = "Unexpected server error",
                Detail = exception.Message,
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }
    }
}
