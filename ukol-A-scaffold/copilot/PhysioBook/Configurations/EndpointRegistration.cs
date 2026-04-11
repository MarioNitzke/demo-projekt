namespace PhysioBook.Configurations;

public static class EndpointRegistration
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapAuthEndpoints();

        api.MapCreateArticleEndpoint();
        api.MapUpdateArticleEndpoint();
        api.MapDeleteArticleEndpoint();
        api.MapGetArticlesEndpoint();
        api.MapGetArticleByIdEndpoint();
    }
}

