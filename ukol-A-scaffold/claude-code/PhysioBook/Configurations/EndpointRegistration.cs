using PhysioBook.Features.Articles.Commands.CreateArticle;
using PhysioBook.Features.Articles.Commands.UpdateArticle;
using PhysioBook.Features.Articles.Commands.DeleteArticle;
using PhysioBook.Features.Articles.Queries.GetArticles;
using PhysioBook.Features.Articles.Queries.GetArticleById;
using PhysioBook.Features.Auth;

namespace PhysioBook.Configurations;

public static class EndpointRegistration
{
    public static void MapEndpoints(this WebApplication app)
    {
        var apiGroup = app.MapGroup("/api");

        var articlesGroup = apiGroup.MapGroup("/articles")
            .WithTags("Articles");

        var authGroup = apiGroup.MapGroup("/auth")
            .WithTags("Auth");

        articlesGroup.MapArticleEndpoints();
        authGroup.MapAuthEndpoints();
    }

    private static RouteGroupBuilder MapArticleEndpoints(this RouteGroupBuilder group)
    {
        group.MapGetArticlesEndpoint();
        group.MapGetArticleByIdEndpoint();
        group.MapCreateArticleEndpoint();
        group.MapUpdateArticleEndpoint();
        group.MapDeleteArticleEndpoint();

        return group;
    }

    private static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapRegisterEndpoint();
        group.MapLoginEndpoint();
        group.MapRefreshTokenEndpoint();

        return group;
    }
}
