using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Finance;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Infrastructure.Services;

public sealed class PaymobGateway : IPaymobGateway
{
    private const int MaxProviderErrorBodyBytes = 8192;
    private const int MaxProviderErrorSummaryLength = 512;

    private readonly HttpClient _httpClient;
    private readonly PaymobSettings _settings;
    private readonly ILogger<PaymobGateway> _logger;

    public PaymobGateway(HttpClient httpClient, IOptions<PaymobSettings> settings,
        ILogger<PaymobGateway> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<Result<PaymobIntentData>> CreateIntentAsync(PaymobIntentCommandData request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey)
            || !Uri.TryCreate(_settings.BaseUrl, UriKind.Absolute, out var baseUri)
            || baseUri.Scheme != Uri.UriSchemeHttps)
            return Result<PaymobIntentData>.Failure(new Error(
                "Finance.PaymobConfigurationInvalid", "Paymob is not configured for secure payment requests."));

        var amountCentsDecimal = request.Amount * 100m;
        if (amountCentsDecimal != decimal.Truncate(amountCentsDecimal)
            || amountCentsDecimal > long.MaxValue || amountCentsDecimal < 1)
            return Result<PaymobIntentData>.Failure(new Error(
                "Finance.PaymobAmountInvalid", "The payment amount cannot be represented in piasters."));

        var amountCents = decimal.ToInt64(amountCentsDecimal);
        var billingName = SplitName(request.FullName);
        var payload = new
        {
            amount = amountCents,
            currency = request.Currency,
            payment_methods = new[] { request.IntegrationId },
            items = new[]
            {
                new { name = request.ItemName, amount = amountCents, description = request.ItemDescription, quantity = 1 }
            },
            billing_data = new
            {
                apartment = "NA", floor = "NA", first_name = billingName.First,
                last_name = billingName.Last, street = "NA", building = "NA",
                phone_number = request.PhoneNumber, shipping_method = "", city = "Cairo",
                country = "EG", state = "Cairo", email = request.Email, postal_code = "NA"
            },
            special_reference = request.SpecialReference,
            expiration = request.ExpirationSeconds,
            notification_url = request.NotificationUrl,
            redirection_url = request.RedirectionUrl
        };

        using var message = new HttpRequestMessage(HttpMethod.Post,
            new Uri(baseUri.ToString().TrimEnd('/') + "/v1/intention/"));
        message.Headers.Authorization = new AuthenticationHeaderValue("Token", _settings.SecretKey);
        message.Content = JsonContent.Create(payload);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Paymob intention request timed out. No provider response body was logged.");
            return Result<PaymobIntentData>.Failure(new Error(
                "Finance.PaymobUnavailable", "Paymob did not return a response. Retry with the same idempotency key."));
        }
        catch (HttpRequestException)
        {
            _logger.LogWarning("Paymob intention request failed at the transport layer.");
            return Result<PaymobIntentData>.Failure(new Error(
                "Finance.PaymobUnavailable", "Paymob is temporarily unavailable. Retry with the same idempotency key."));
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await ReadBoundedBodyAsync(response.Content, cancellationToken);
                var providerError = SummarizeProviderError(responseBody, request, _settings.SecretKey);
                _logger.LogWarning("Paymob intention request returned HTTP {StatusCode}. Provider error: {ProviderError}",
                    (int)response.StatusCode, providerError);
                var code = response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden
                    ? "Finance.PaymobCredentialsRejected"
                    : response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound or HttpStatusCode.UnprocessableEntity
                        ? "Finance.PaymobRequestRejected"
                        : "Finance.PaymobUnavailable";
                return Result<PaymobIntentData>.Failure(new Error(code,
                    "Paymob could not create the payment intention. Check the integration configuration and retry with the same idempotency key."));
            }

            try
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
                var root = document.RootElement;
                var intentionId = GetString(root, "id");
                var orderId = GetScalarString(root, "intention_order_id")
                    ?? GetNestedOrderId(root);
                var clientSecret = GetString(root, "client_secret");
                if (string.IsNullOrWhiteSpace(intentionId) || string.IsNullOrWhiteSpace(orderId)
                    || string.IsNullOrWhiteSpace(clientSecret))
                {
                    _logger.LogError("Paymob returned an incomplete intention response.");
                    return Result<PaymobIntentData>.Failure(new Error(
                        "Finance.PaymobInvalidResponse", "Paymob returned an incomplete payment response."));
                }

                return Result<PaymobIntentData>.Success(new PaymobIntentData(intentionId, orderId, clientSecret));
            }
            catch (JsonException)
            {
                _logger.LogError("Paymob returned an invalid JSON intention response.");
                return Result<PaymobIntentData>.Failure(new Error(
                    "Finance.PaymobInvalidResponse", "Paymob returned an invalid payment response."));
            }
        }
    }

    private static (string First, string Last) SplitName(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length switch
        {
            0 => ("Student", "Account"),
            1 => (parts[0], "Student"),
            _ => (parts[0], string.Join(' ', parts.Skip(1)))
        };
    }

    private static async Task<string> ReadBoundedBodyAsync(HttpContent content, CancellationToken cancellationToken)
    {
        await using var stream = await content.ReadAsStreamAsync(cancellationToken);
        using var buffer = new MemoryStream();
        var chunk = new byte[1024];
        while (buffer.Length < MaxProviderErrorBodyBytes)
        {
            var remaining = MaxProviderErrorBodyBytes - (int)buffer.Length;
            var read = await stream.ReadAsync(chunk.AsMemory(0, Math.Min(chunk.Length, remaining)), cancellationToken);
            if (read == 0) break;
            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    private static string SummarizeProviderError(string responseBody, PaymobIntentCommandData request, string secretKey)
    {
        var messages = new List<string>();
        try
        {
            using var document = JsonDocument.Parse(responseBody);
            CollectErrorMessages(document.RootElement, messages, 0, collectNestedStrings: false);
        }
        catch (JsonException)
        {
            // The bounded response text is sanitized below before it is written to logs.
        }

        var summary = messages.Count > 0
            ? string.Join(" | ", messages.Distinct(StringComparer.Ordinal))
            : responseBody.Trim();
        if (string.IsNullOrWhiteSpace(summary)) return "Provider returned an empty error response.";

        var sensitiveValues = new List<string>
            { secretKey, request.Email, request.PhoneNumber, request.FullName, request.SpecialReference };
        sensitiveValues.AddRange(request.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        foreach (var sensitiveValue in sensitiveValues)
        {
            if (!string.IsNullOrWhiteSpace(sensitiveValue))
                summary = summary.Replace(sensitiveValue, "[REDACTED]", StringComparison.OrdinalIgnoreCase);
        }

        summary = summary.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');

        return summary.Length <= MaxProviderErrorSummaryLength
            ? summary
            : summary[..MaxProviderErrorSummaryLength];
    }

    private static void CollectErrorMessages(JsonElement element, List<string> messages, int depth,
        bool collectNestedStrings)
    {
        if (depth > 4 || messages.Count >= 8) return;

        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                var isErrorField = property.Name.Equals("detail", StringComparison.OrdinalIgnoreCase)
                    || property.Name.Equals("message", StringComparison.OrdinalIgnoreCase)
                    || property.Name.Equals("error", StringComparison.OrdinalIgnoreCase)
                    || property.Name.Equals("error_description", StringComparison.OrdinalIgnoreCase)
                    || property.Name.Equals("code", StringComparison.OrdinalIgnoreCase)
                    || property.Name.Equals("errors", StringComparison.OrdinalIgnoreCase);
                if (isErrorField || collectNestedStrings)
                {
                    CollectErrorMessages(property.Value, messages, depth + 1, collectNestedStrings: true);
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                CollectErrorMessages(item, messages, depth + 1, collectNestedStrings);
                if (messages.Count >= 8) break;
            }
        }
        else if (element.ValueKind == JsonValueKind.String && collectNestedStrings)
        {
            var value = element.GetString();
            if (!string.IsNullOrWhiteSpace(value)) messages.Add(value.Trim());
        }
    }

    private static string? GetString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() : null;

    private static string? GetScalarString(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value)) return null;
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.GetRawText(),
            _ => null
        };
    }

    private static string? GetNestedOrderId(JsonElement element)
    {
        if (!element.TryGetProperty("payment_keys", out var keys) || keys.ValueKind != JsonValueKind.Array) return null;
        foreach (var key in keys.EnumerateArray())
        {
            var orderId = GetScalarString(key, "order_id");
            if (!string.IsNullOrWhiteSpace(orderId)) return orderId;
        }
        return null;
    }
}

public sealed class PaymobCallbackVerifier : IPaymobCallbackVerifier
{
    private static readonly string[] HmacFields =
    [
        "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
        "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
        "is_standalone_payment", "is_voided", "order.id", "owner", "pending",
        "source_data.pan", "source_data.sub_type", "source_data.type", "success"
    ];

    private readonly PaymobSettings _settings;

    public PaymobCallbackVerifier(IOptions<PaymobSettings> settings) => _settings = settings.Value;

    public bool IsValidTransactionHmac(JsonElement transactionObject, string? suppliedHmac)
    {
        if (string.IsNullOrWhiteSpace(_settings.HmacSecret) || string.IsNullOrWhiteSpace(suppliedHmac)
            || suppliedHmac.Length != 128 || transactionObject.ValueKind != JsonValueKind.Object)
            return false;

        var input = new StringBuilder(512);
        foreach (var path in HmacFields)
        {
            if (!TryGetHmacValue(transactionObject, path, out var value)) return false;
            input.Append(value);
        }

        byte[] supplied;
        try { supplied = Convert.FromHexString(suppliedHmac); }
        catch (FormatException) { return false; }

        var expected = HMACSHA512.HashData(
            Encoding.UTF8.GetBytes(_settings.HmacSecret), Encoding.UTF8.GetBytes(input.ToString()));
        return CryptographicOperations.FixedTimeEquals(expected, supplied);
    }

    private bool TryGetHmacValue(JsonElement root, string path, out string value)
    {
        value = string.Empty;
        var parts = path.Split('.');
        var current = root;
        foreach (var part in parts)
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(part, out current)) return false;
        }

        if (current.ValueKind == JsonValueKind.Null)
        {
            value = _settings.HmacNullValue;
            return true;
        }
        value = current.ValueKind switch
        {
            JsonValueKind.String => current.GetString() ?? string.Empty,
            JsonValueKind.Number => current.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => string.Empty
        };
        return current.ValueKind is JsonValueKind.String or JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False;
    }
}
