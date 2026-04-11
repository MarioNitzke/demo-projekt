namespace PhysioBook.Auth;

public interface IJwtTokenService
{
    Task<AuthTokenResult> CreateTokenAsync(AppUser user);
    string GenerateRefreshToken();
}

public sealed record AuthTokenResult(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtAuthSettings _settings;
    private readonly UserManager<AppUser> _userManager;

    public JwtTokenService(IOptions<JwtAuthSettings> options, UserManager<AppUser> userManager)
    {
        _settings = options.Value;
        _userManager = userManager;
    }

    public async Task<AuthTokenResult> CreateTokenAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        return new AuthTokenResult(accessToken, refreshToken, expiresAt);
    }

    public string GenerateRefreshToken() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}

