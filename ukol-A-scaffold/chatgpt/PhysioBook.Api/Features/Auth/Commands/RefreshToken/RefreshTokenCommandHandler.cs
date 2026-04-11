namespace PhysioBook.Api.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService)
    : IQueryHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(
            x => x.RefreshToken == request.RefreshToken,
            cancellationToken);

        if (user is null || user.RefreshTokenExpiresAtUtc is null || user.RefreshTokenExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token is invalid or expired.");
        }

        var tokens = await _jwtTokenService.CreateTokensAsync(user, cancellationToken);
        return user.ToResponse(tokens);
    }
}
