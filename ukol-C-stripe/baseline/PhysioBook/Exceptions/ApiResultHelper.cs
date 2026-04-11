using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace PhysioBook.Exceptions;

public static class ApiResultHelper
{
    public static async Task<IResult> ExecuteWithErrorHandling(Func<Task<IResult>> handler)
    {
        try
        {
            return await handler();
        }
        catch (ValidationException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Detail = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))
            };
            return Results.Problem(problemDetails);
        }
        catch (UnauthorizedAccessException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Title = "Unauthorized",
                Detail = ex.Message
            };
            return Results.Problem(problemDetails);
        }
        catch (KeyNotFoundException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "Not Found",
                Detail = ex.Message
            };
            return Results.Problem(problemDetails);
        }
        catch (ArgumentException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Bad Request",
                Detail = ex.Message
            };
            return Results.Problem(problemDetails);
        }
        catch (BookingConflictException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "Conflict",
                Detail = ex.Message
            };
            return Results.Conflict(problemDetails);
        }
        catch (Exception)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please try again later."
            };
            return Results.Problem(problemDetails);
        }
    }
}
