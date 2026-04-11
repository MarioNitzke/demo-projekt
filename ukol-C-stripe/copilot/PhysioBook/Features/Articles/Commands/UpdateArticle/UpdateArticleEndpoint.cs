using PhysioBook.Exceptions;

namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public static class UpdateArticleEndpoint
{
    public static RouteGroupBuilder MapUpdateArticleEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (Guid id, UpdateArticleRequest request, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand(id);

                var response = await mediator.SendAsync<UpdateArticleCommand, UpdateArticleResponse>(command, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization("Admin")
        .Produces<UpdateArticleResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("UpdateArticle");

        return group;
    }
}
