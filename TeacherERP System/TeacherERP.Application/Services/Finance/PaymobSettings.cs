namespace TeacherERP.Application.Finance;

/// <summary>Configuration contract for the Paymob adapter. Secrets are supplied by the host environment.</summary>
public sealed class PaymobSettings
{
    public string Mode { get; set; } = "Test";
    public string BaseUrl { get; set; } = "https://accept.paymob.com";
    public string Currency { get; set; } = "EGP";
    public string ApiKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string HmacSecret { get; set; } = string.Empty;
    public string NotificationUrl { get; set; } = string.Empty;
    public string RedirectionUrl { get; set; } = string.Empty;
    /// <summary>Null serialization must be confirmed against this merchant's Paymob callback samples.</summary>
    public string HmacNullValue { get; set; } = string.Empty;
    public int IntentionExpirationSeconds { get; set; } = 1800;
    public int InitializationLeaseSeconds { get; set; } = 120;
    public Dictionary<string, int> IntegrationIds { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
