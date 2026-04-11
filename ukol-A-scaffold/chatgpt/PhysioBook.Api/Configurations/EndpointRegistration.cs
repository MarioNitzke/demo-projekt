using PhysioBook.Api.Features.Articles.Commands.CreateArticle;
using PhysioBook.Api.Features.Articles.Commands.DeleteArticle;
using PhysioBook.Api.Features.Articles.Commands.UpdateArticle;
using PhysioBook.Api.Features.Articles.Queries.GetArticleById;
using PhysioBook.Api.Features.Articles.Queries.GetArticles;
using PhysioBook.Api.Features.Auth.Commands.Login;
using PhysioBook.Api.Features.Auth.Commands.RefreshToken;
using PhysioBook.Api.Features.Auth.Commands.Register;

namespace PhysioBook.Api.Configurations;

public static class EndpointRegistration
{
    public static WebApplication RegisterEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new
        {
            name = "PhysioBook API",
            version = "v1",
            status = "ok"
        }))
        .AllowAnonymous();

        app.MapRegisterEndpoint();
        app.MapLoginEndpoint();
        app.MapRefreshTokenEndpoint();

        app.MapGetArticlesEndpoint();
        app.MapGetArticleByIdEndpoint();
        app.MapCreateArticleEndpoint();
        app.MapUpdateArticleEndpoint();
        app.MapDeleteArticleEndpoint();

        return app;
    }
}
