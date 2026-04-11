namespace PhysioBook.Api.Auth;

public sealed class JwtTokenService(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtAuthSettings> jwtOptions)
    : IJwtTokenService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtAuthSettings _settings = jwtOptions.Value;

    public async Task<TokenResult> CreateTokensAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var now = DateTime.UtcNow;
        var accessTokenExpiresAt = now.AddMinutes(_settings.AccessTokenExpirationMinutes);
        var refreshTokenExpiresAt = now.AddDays(_settings.RefreshTokenExpirationDays);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: accessTokenExpiresAt,
            signingCredentials: signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpiresAt;
        await _userManager.UpdateAsync(user);

        return new TokenResult(
            accessToken,
            accessTokenExpiresAt,
            refreshToken,
            refreshTokenExpiresAt,
            roles.ToArray());
    }
}
