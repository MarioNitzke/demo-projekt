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

public record RefreshTokenRequest(string Token, string RefreshToken);
public record RefreshTokenResponse(string Token, string RefreshToken, DateTime Expiration);

public static class RefreshTokenEndpoint
{
    public static RouteGroupBuilder MapRefreshTokenEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/refresh-token", async (
            RefreshTokenRequest request,
            UserManager<ApplicationUser> userManager,
            IOptions<JwtAuthSettings> jwtOptions,
            CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var jwtSettings = jwtOptions.Value;

                var principal = GetPrincipalFromExpiredToken(request.Token, jwtSettings);
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new UnauthorizedAccessException("Invalid token.");

                var user = await userManager.FindByIdAsync(userId)
                    ?? throw new UnauthorizedAccessException("Invalid token.");

                if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    throw new UnauthorizedAccessException("Invalid or expired refresh token.");
                }

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

                var newRefreshToken = Guid.NewGuid().ToString();
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationInDays);
                await userManager.UpdateAsync(user);

                return Results.Ok(new RefreshTokenResponse(tokenString, newRefreshToken, token.ValidTo));
            });
        })
        .AllowAnonymous()
        .Produces<RefreshTokenResponse>()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithName("RefreshToken");

        return group;
    }

    private static ClaimsPrincipal GetPrincipalFromExpiredToken(string token, JwtAuthSettings jwtSettings)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedAccessException("Invalid token.");
        }

        return principal;
    }
}
