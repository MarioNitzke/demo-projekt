namespace PhysioBook.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IQueryHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _tokenService;
    private readonly IOptions<JwtAuthSettings> _jwtSettings;

    public LoginCommandHandler(UserManager<AppUser> userManager, IJwtTokenService tokenService, IOptions<JwtAuthSettings> jwtSettings)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings;
    }

    public async Task<LoginResponse> Handle(LoginCommand query, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(query.Email);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var isValid = await _userManager.CheckPasswordAsync(user, query.Password);
        if (!isValid)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var tokens = await _tokenService.CreateTokenAsync(user);
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiryTimeUtc = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenDays);
        await _userManager.UpdateAsync(user);

        return new LoginResponse(tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresAtUtc);
    }
}

