using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PhysioBook.Exceptions;

public class GlobalExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            ValidationException validationException => (
                (int)HttpStatusCode.BadRequest,
                "Validation Error",
                string.Join("; ", validationException.Errors.Select(e => e.ErrorMessage))
            ),
            UnauthorizedAccessException => (
                (int)HttpStatusCode.Unauthorized,
                "Unauthorized",
                "You are not authorized to perform this action."
            ),
            KeyNotFoundException => (
                (int)HttpStatusCode.NotFound,
                "Not Found",
                exception.Message
            ),
            ArgumentException => (
                (int)HttpStatusCode.BadRequest,
                "Bad Request",
                exception.Message
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later."
            )
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
