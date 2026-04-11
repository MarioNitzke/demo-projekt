using PhysioBook.Exceptions;

namespace PhysioBook.Features.Articles.Commands.DeleteArticle;

public static class DeleteArticleEndpoint
{
    public static RouteGroupBuilder MapDeleteArticleEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = new DeleteArticleRequest(id).ToCommand();

                var response = await mediator.SendAsync<DeleteArticleCommand, DeleteArticleResponse>(command, ct);

                return Results.Ok(response);
            });
        })
        .RequireAuthorization("Admin")
        .Produces<DeleteArticleResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("DeleteArticle");

        return group;
    }
}
