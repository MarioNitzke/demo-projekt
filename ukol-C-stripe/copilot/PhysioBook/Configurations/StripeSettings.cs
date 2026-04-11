namespace PhysioBook.Configurations;

public class StripeSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string Currency { get; set; } = "czk";
    public string FrontendBaseUrl { get; set; } = "http://localhost:5173";
}

