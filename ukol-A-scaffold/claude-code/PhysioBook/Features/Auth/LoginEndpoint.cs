using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PhysioBook.Auth;
using PhysioBook.Data.Entities;
using PhysioBook.Exceptions;

namespace PhysioBook.Features.Auth;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string RefreshToken, DateTime Expiration);

public static class LoginEndpoint
{
    public static RouteGroupBuilder MapLoginEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/login", async (
            LoginRequest request,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtAuthSettings> jwtOptions,
            CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var user = await userManager.FindByEmailAsync(request.Email)
                    ?? throw new UnauthorizedAccessException("Invalid email or password.");

                var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (!result.Succeeded)
                {
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                var jwtSettings = jwtOptions.Value;
                var roles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id),
                    new(ClaimTypes.Email, user.Email!),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var role in roles)
                    authClaims.Add(new Claim(ClaimTypes.Role, role));

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));

                var token = new JwtSecurityToken(
                    issuer: jwtSettings.Issuer,
                    audience: jwtSettings.Audience,
                    expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationInMinutes),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var refreshToken = Guid.NewGuid().ToString();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationInDays);
                await userManager.UpdateAsync(user);

                return Results.Ok(new LoginResponse(tokenString, refreshToken, token.ValidTo));
            });
        })
        .AllowAnonymous()
        .Produces<LoginResponse>()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithName("Login");

        return group;
    }
}
