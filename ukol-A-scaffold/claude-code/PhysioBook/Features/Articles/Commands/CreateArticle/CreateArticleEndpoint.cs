using System.Security.Claims;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public static class CreateArticleEndpoint
{
    public static RouteGroupBuilder MapCreateArticleEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreateArticleRequest request, IMediator mediator, HttpContext httpContext, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();
                command.AuthorId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var response = await mediator.SendAsync<CreateArticleCommand, CreateArticleResponse>(command, ct);

                return Results.Created($"/api/articles/{response.Id}", response);
            });
        })
        .RequireAuthorization("Admin")
        .Produces<CreateArticleResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("CreateArticle");

        return group;
    }
}
