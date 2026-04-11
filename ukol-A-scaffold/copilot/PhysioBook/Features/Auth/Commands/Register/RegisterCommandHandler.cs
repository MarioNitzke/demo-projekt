namespace PhysioBook.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : IQueryHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand query, CancellationToken cancellationToken = default)
    {
        var existing = await _userManager.FindByEmailAsync(query.Email);
        if (existing is not null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        var user = new AppUser
        {
            UserName = query.Email,
            Email = query.Email
        };

        var created = await _userManager.CreateAsync(user, query.Password);
        if (!created.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", created.Errors.Select(x => x.Description)));
        }

        await _userManager.AddToRoleAsync(user, query.Role);
        return new RegisterResponse(user.Id, user.Email ?? string.Empty);
    }
}

