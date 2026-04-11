namespace PhysioBook.Exceptions;

public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch
        {
            // Endpointy primarne pouzivaji ApiResultHelper; middleware je fallback.
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Unexpected error"
                });
            }
        }
    }
}

