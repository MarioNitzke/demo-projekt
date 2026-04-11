namespace PhysioBook.Api.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService)
    : IQueryHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = request.ToEntity();

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        await _userManager.AddToRoleAsync(user, AppRoles.Admin);

        var tokens = await _jwtTokenService.CreateTokensAsync(user, cancellationToken);
        return user.ToResponse(tokens);
    }
}
