namespace PhysioBook.Exceptions;

public static class ApiResultHelper
{
    public static async Task<IResult> ExecuteWithErrorHandling(Func<Task<IResult>> action)
    {
        try
        {
            return await action();
        }
        catch (ValidationException ex)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation failed",
                detail: string.Join("; ", ex.Errors.Select(x => x.ErrorMessage)));
        }
        catch (KeyNotFoundException ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Resource not found", detail: ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Unauthorized", detail: ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid operation", detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Unexpected error", detail: ex.Message);
        }
    }
}

