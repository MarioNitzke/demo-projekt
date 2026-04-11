namespace PhysioBook.Data.Entities;

public sealed class AppUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTimeUtc { get; set; }
}

