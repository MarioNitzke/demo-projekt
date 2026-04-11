namespace PhysioBook.Api.Exceptions;

public static class ApiResultHelper
{
    public static async Task<IResult> ExecuteWithErrorHandling(Func<Task<IResult>> action)
    {
        try
        {
            return await action();
        }
        catch (ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    group => ToCamelCase(group.Key),
                    group => group.Select(x => x.ErrorMessage).Distinct().ToArray());

            return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (KeyNotFoundException exception)
        {
            return Results.Problem(
                title: "Resource not found",
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Results.Problem(
                title: "Unauthorized",
                detail: exception.Message,
                statusCode: StatusCodes.Status401Unauthorized);
        }
        catch (InvalidOperationException exception)
        {
            return Results.Problem(
                title: "Invalid operation",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (Exception exception)
        {
            return Results.Problem(
                title: "Unexpected server error",
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
