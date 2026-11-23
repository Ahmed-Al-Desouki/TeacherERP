using System.Text.Json;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces;

public enum PaymobIntegrationKey
{
    CaggOnlineNonShopify,
    CashCashChannel,
    UigOnlineNewNonShopify,
    VpcOnlineNonShopify,
    VpcShopify
}

public sealed record CreatePaymobPaymentRequest(
    Guid SessionId,
    PaymentType Type,
    decimal Amount,
    PaymobIntegrationKey Integration);

public sealed record PaymobIntentCommandData(
    decimal Amount,
    string Currency,
    int ExpirationSeconds,
    int IntegrationId,
    string SpecialReference,
    string NotificationUrl,
    string? RedirectionUrl,
    string FullName,
    string Email,
    string PhoneNumber,
    string ItemName,
    string ItemDescription);

public sealed record PaymobIntentData(string IntentionId, string OrderId, string ClientSecret);

public sealed record PaymobPaymentIntentResult(
    Guid TransactionId,
    TransactionStatus Status,
    PaymobIntegrationKey Integration,
    decimal Amount,
    string Currency,
    string? IntentionId,
    string? OrderId,
    string? ClientSecret,
    string? PublicKey,
    bool IsProcessing,
    string? ErrorCode);

public enum PaymobWebhookOutcome { Accepted, Duplicate, Ignored, InvalidSignature, InvalidPayload }

public interface IPaymobPaymentService
{
    Task<Result<PaymobPaymentIntentResult>> CreateIntentAsync(
        Guid tenantId, Guid studentId, string idempotencyKey, CreatePaymobPaymentRequest request,
        CancellationToken cancellationToken);

    Task<PaymobWebhookOutcome> HandleTransactionCallbackAsync(
        JsonElement root, string? hmac, CancellationToken cancellationToken);
}

public interface IPaymobGateway
{
    Task<Result<PaymobIntentData>> CreateIntentAsync(PaymobIntentCommandData request,
        CancellationToken cancellationToken);
}

public interface IPaymobCallbackVerifier
{
    bool IsValidTransactionHmac(JsonElement transactionObject, string? suppliedHmac);
}
