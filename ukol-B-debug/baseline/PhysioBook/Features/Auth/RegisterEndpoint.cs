using Microsoft.AspNetCore.Identity;
using PhysioBook.Data.Entities;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Auth;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public record RegisterResponse(string UserId, string Email);

public static class RegisterEndpoint
{
    public static RouteGroupBuilder MapRegisterEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (RegisterRequest request, UserManager<ApplicationUser> userManager, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                var result = await userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new ArgumentException(errors);
                }

                return Results.Ok(new RegisterResponse(user.Id, user.Email!));
            });
        })
        .AllowAnonymous()
        .Produces<RegisterResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("Register");

        return group;
    }
}
