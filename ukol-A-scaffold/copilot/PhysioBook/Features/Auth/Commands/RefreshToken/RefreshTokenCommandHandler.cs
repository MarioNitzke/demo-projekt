namespace PhysioBook.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IQueryHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _tokenService;
    private readonly IOptions<JwtAuthSettings> _jwtSettings;

    public RefreshTokenCommandHandler(UserManager<AppUser> userManager, IJwtTokenService tokenService, IOptions<JwtAuthSettings> jwtSettings)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand query, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(query.Email);
        if (user is null || user.RefreshToken != query.RefreshToken || user.RefreshTokenExpiryTimeUtc < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var tokens = await _tokenService.CreateTokenAsync(user);
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiryTimeUtc = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenDays);
        await _userManager.UpdateAsync(user);

        return new RefreshTokenResponse(tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresAtUtc);
    }
}

