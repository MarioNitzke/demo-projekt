namespace PhysioBook.Api.Data.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }
}
