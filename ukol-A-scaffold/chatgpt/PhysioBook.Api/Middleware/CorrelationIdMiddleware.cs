namespace PhysioBook.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existingCorrelationId)
            ? existingCorrelationId.ToString()
            : Guid.NewGuid().ToString("N");

        context.Items[HeaderName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        await _next(context);
    }
}
