using PhysioBook.Features.Articles.Commands.CreateArticle;
using PhysioBook.Features.Articles.Commands.UpdateArticle;
using PhysioBook.Features.Articles.Commands.DeleteArticle;
using PhysioBook.Features.Articles.Queries.GetArticles;
using PhysioBook.Features.Articles.Queries.GetArticleById;
using PhysioBook.Features.Auth;
using PhysioBook.Features.Services.Commands.CreateService;
using PhysioBook.Features.Services.Commands.UpdateService;
using PhysioBook.Features.Services.Commands.DeleteService;
using PhysioBook.Features.Services.Queries.GetServices;
using PhysioBook.Features.Services.Queries.GetServiceById;
using PhysioBook.Features.TimeSlots.Commands.CreateTimeSlot;
using PhysioBook.Features.TimeSlots.Commands.UpdateTimeSlot;
using PhysioBook.Features.TimeSlots.Commands.DeleteTimeSlot;
using PhysioBook.Features.TimeSlots.Queries.GetTimeSlots;
using PhysioBook.Features.Bookings.Commands.CreateBooking;
using PhysioBook.Features.Bookings.Commands.CancelBooking;
using PhysioBook.Features.Bookings.Queries.GetBookings;
using PhysioBook.Features.Bookings.Queries.GetBookingById;
using PhysioBook.Features.Bookings.Queries.GetAvailability;

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

        var servicesGroup = apiGroup.MapGroup("/services")
            .WithTags("Services");

        var timeSlotsGroup = apiGroup.MapGroup("/timeslots")
            .WithTags("TimeSlots");

        var bookingsGroup = apiGroup.MapGroup("/bookings")
            .WithTags("Bookings");

        articlesGroup.MapArticleEndpoints();
        authGroup.MapAuthEndpoints();
        servicesGroup.MapServiceEndpoints();
        timeSlotsGroup.MapTimeSlotEndpoints();
        bookingsGroup.MapBookingEndpoints();
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

    private static RouteGroupBuilder MapServiceEndpoints(this RouteGroupBuilder group)
    {
        group.MapGetServicesEndpoint();
        group.MapGetServiceByIdEndpoint();
        group.MapCreateServiceEndpoint();
        group.MapUpdateServiceEndpoint();
        group.MapDeleteServiceEndpoint();

        return group;
    }

    private static RouteGroupBuilder MapTimeSlotEndpoints(this RouteGroupBuilder group)
    {
        group.MapGetTimeSlotsEndpoint();
        group.MapCreateTimeSlotEndpoint();
        group.MapUpdateTimeSlotEndpoint();
        group.MapDeleteTimeSlotEndpoint();

        return group;
    }

    private static RouteGroupBuilder MapBookingEndpoints(this RouteGroupBuilder group)
    {
        group.MapGetAvailabilityEndpoint();
        group.MapGetBookingsEndpoint();
        group.MapGetBookingByIdEndpoint();
        group.MapCreateBookingEndpoint();
        group.MapCancelBookingEndpoint();

        return group;
    }
}
