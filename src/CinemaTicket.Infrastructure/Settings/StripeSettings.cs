namespace CinemaTicket.Infrastructure.Settings;

/// <summary>
/// Configuration settings for Stripe payment integration.
/// </summary>
public class StripeSettings
{
    /// <summary>
    /// Gets or sets the Stripe API secret key (starts with sk_test_ for test mode or sk_live_ for production).
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Stripe publishable key (starts with pk_test_ for test mode or pk_live_ for production).
    /// Used on the client-side for tokenizing payment methods.
    /// </summary>
    public string PublishableKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the webhook secret for verifying Stripe webhook signatures.
    /// Starts with whsec_ and is obtained from the Stripe Dashboard.
    /// </summary>
    public string WebhookSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default currency for payments (e.g., "usd", "eur", "gbp").
    /// Must be a three-letter ISO currency code in lowercase.
    /// Defaults to "usd".
    /// </summary>
    public string Currency { get; set; } = "usd";
}
