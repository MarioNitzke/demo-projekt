namespace PhysioBook.Api.Auth;

public interface IJwtTokenService
{
    Task<TokenResult> CreateTokensAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}
