namespace PhysioBook.Api.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService)
    : IQueryHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var tokens = await _jwtTokenService.CreateTokensAsync(user, cancellationToken);
        return user.ToResponse(tokens);
    }
}
