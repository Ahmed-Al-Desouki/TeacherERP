using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Finance;

public sealed class PaymobPaymentService : IPaymobPaymentService
{
    private readonly IFinancialLedgerRepository _repository;
    private readonly IFinancialLedgerService _ledger;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditRepository _auditRepository;
    private readonly IPaymobGateway _gateway;
    private readonly IPaymobCallbackVerifier _callbackVerifier;
    private readonly PaymobSettings _settings;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<PaymobPaymentService> _logger;

    public PaymobPaymentService(IFinancialLedgerRepository repository, IFinancialLedgerService ledger,
        IUserRepository users, IUnitOfWork unitOfWork, IAuditRepository auditRepository,
        IPaymobGateway gateway, IPaymobCallbackVerifier callbackVerifier,
        IOptions<PaymobSettings> settings, TimeProvider timeProvider, ILogger<PaymobPaymentService> logger)
    {
        _repository = repository;
        _ledger = ledger;
        _users = users;
        _unitOfWork = unitOfWork;
        _auditRepository = auditRepository;
        _gateway = gateway;
        _callbackVerifier = callbackVerifier;
        _settings = settings.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<Result<PaymobPaymentIntentResult>> CreateIntentAsync(
        Guid tenantId, Guid studentId, string idempotencyKey, CreatePaymobPaymentRequest request,
        CancellationToken cancellationToken)
    {
        idempotencyKey = idempotencyKey?.Trim() ?? string.Empty;
        if (tenantId == Guid.Empty || studentId == Guid.Empty || idempotencyKey.Length is < 8 or > 128)
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.InvalidIdempotencyKey);
        if (!Enum.IsDefined(request.Type) || !Enum.IsDefined(request.Integration))
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.UnsupportedPaymentMethod);
        if (request.Amount <= 0 || decimal.Round(request.Amount, 2) != request.Amount
            || request.Amount * 100m > long.MaxValue)
            return Result<PaymobPaymentIntentResult>.Failure(FinanceErrors.InvalidAmount);

        var integrationId = GetIntegrationId(request.Integration);
        if (request.Integration == PaymobIntegrationKey.VpcShopify)
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.ShopifyIntegrationNotSupported);
        if (integrationId <= 0)
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.IntegrationNotConfigured);
        if (string.IsNullOrWhiteSpace(_settings.SecretKey) || string.IsNullOrWhiteSpace(_settings.PublicKey)
            || string.IsNullOrWhiteSpace(_settings.HmacSecret))
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.ConfigurationInvalid);
        if (!IsPublicHttpsUrl(_settings.NotificationUrl))
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.WebhookUrlRequired);
        if (request.Integration != PaymobIntegrationKey.CashCashChannel
            && string.IsNullOrWhiteSpace(_settings.RedirectionUrl))
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.RedirectionUrlRequired);
        if (!string.IsNullOrWhiteSpace(_settings.RedirectionUrl)
            && !IsHttpsUrl(_settings.RedirectionUrl))
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.RedirectionUrlInvalid);

        var optionsResult = await _ledger.GetOptionsAsync(tenantId, studentId, request.SessionId, cancellationToken);
        if (optionsResult.IsFailure) return Result<PaymobPaymentIntentResult>.Failure(optionsResult.Error);
        var options = optionsResult.Value!;
        var validationError = ValidatePayment(options, request.Type, request.Amount);
        if (validationError is not null) return Result<PaymobPaymentIntentResult>.Failure(validationError);

        var user = await _users.GetByIdAsync(studentId, cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PhoneNumber))
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.BillingProfileIncomplete);

        var fingerprint = Fingerprint(tenantId, studentId, request, integrationId);
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var lease = TimeSpan.FromSeconds(Math.Clamp(_settings.InitializationLeaseSeconds, 15, 600));
        Transaction? transactionToInitialize = null;
        PaymobPaymentIntentResult? readyResult = null;
        Error? setupError = null;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existing = await _repository.GetPaymobPaymentByIdempotencyAsync(
                tenantId, studentId, idempotencyKey, cancellationToken);
            if (existing is not null)
            {
                if (!CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(existing.RequestFingerprint ?? string.Empty),
                        Encoding.UTF8.GetBytes(fingerprint)))
                {
                    setupError = PaymobErrors.IdempotencyKeyReused;
                    return true;
                }

                if (existing.Status == TransactionStatus.Failed)
                {
                    setupError = new Error(existing.ProviderFailureCode ?? PaymobErrors.IntentCreationFailed.Code,
                        "This idempotent payment attempt has already failed. Create a new key for a new attempt.");
                    return true;
                }

                if (existing.Status != TransactionStatus.PaymobPending
                    || !string.IsNullOrWhiteSpace(existing.ProviderClientSecret))
                {
                    readyResult = ToResult(existing, request.Integration, isProcessing: false);
                    return true;
                }

                if (!existing.TryBeginPaymobInitialization(now, lease))
                {
                    readyResult = ToResult(existing, request.Integration, isProcessing: true);
                    return true;
                }

                transactionToInitialize = existing;
                _repository.AddPaymentEvent(PaymentEvent.Create(tenantId, existing.Id, PaymentProvider.Paymob,
                    $"intent-retry:{existing.Id:N}:{now.Ticks}", "intention-request-retried",
                    existing.Status, existing.Status));
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }

            if (await _repository.HasOpenExternalPaymentAsync(tenantId, studentId, request.Type,
                    request.SessionId, cancellationToken))
            {
                setupError = PaymobErrors.PaymentAlreadyPending;
                return true;
            }

            var payment = Transaction.CreatePaymobPayment(tenantId, studentId, request.Amount,
                request.Type, request.SessionId, idempotencyKey, fingerprint, integrationId, now);
            _repository.AddPayment(payment);
            _repository.AddPaymentEvent(PaymentEvent.Create(tenantId, payment.Id, PaymentProvider.Paymob,
                $"intent-requested:{payment.Id:N}", "intention-requested", null, payment.Status));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            transactionToInitialize = payment;
            return true;
        }, IsolationLevel.Serializable, cancellationToken);

        if (setupError is not null) return Result<PaymobPaymentIntentResult>.Failure(setupError);
        if (transactionToInitialize is null)
            return Result<PaymobPaymentIntentResult>.Success(readyResult!);

        var commandData = new PaymobIntentCommandData(
            request.Amount, _settings.Currency, Math.Clamp(_settings.IntentionExpirationSeconds, 60, 86400),
            integrationId, transactionToInitialize.Id.ToString("D"), _settings.NotificationUrl,
            request.Integration == PaymobIntegrationKey.CashCashChannel ? null : _settings.RedirectionUrl,
            user.FullName, user.Email!, user.PhoneNumber!, GetItemName(request.Type),
            $"TeacherERP {request.Type} payment");

        var providerResult = await _gateway.CreateIntentAsync(commandData, cancellationToken);
        if (providerResult.IsFailure)
        {
            var failureCode = providerResult.Error.Code;
            var outcomeIsUncertain = failureCode is not (
                "Finance.PaymobCredentialsRejected" or "Finance.PaymobRequestRejected"
                or "Finance.PaymobConfigurationInvalid" or "Finance.PaymobAmountInvalid");
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var tracked = await _repository.GetTransactionTrackedAsync(tenantId,
                    transactionToInitialize.Id, cancellationToken);
                if (tracked is not null && tracked.Status == TransactionStatus.PaymobPending)
                {
                    var previous = tracked.Status;
                    var current = previous;
                    if (!outcomeIsUncertain && tracked.MarkPaymobInitializationFailed(failureCode))
                        current = tracked.Status;
                    _repository.AddPaymentEvent(PaymentEvent.Create(tenantId, tracked.Id, PaymentProvider.Paymob,
                        $"intent-attempt:{tracked.Id:N}:{Guid.NewGuid():N}",
                        outcomeIsUncertain ? "intention-creation-outcome-uncertain" : "intention-creation-failed",
                        previous, current));
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                return true;
            }, IsolationLevel.Serializable, cancellationToken);
            return Result<PaymobPaymentIntentResult>.Failure(providerResult.Error);
        }

        var created = providerResult.Value!;
        var persisted = false;
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var tracked = await _repository.GetTransactionTrackedAsync(tenantId,
                transactionToInitialize.Id, cancellationToken);
            if (tracked is null || tracked.Status != TransactionStatus.PaymobPending) return false;
            tracked.AttachPaymobIntention(created.IntentionId, created.OrderId, created.ClientSecret);
            _repository.AddPaymentEvent(PaymentEvent.Create(tenantId, tracked.Id, PaymentProvider.Paymob,
                $"intention-created:{created.IntentionId}", "intention-created",
                TransactionStatus.PaymobPending, TransactionStatus.PaymobPending, created.OrderId));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            persisted = true;
            return true;
        }, IsolationLevel.Serializable, cancellationToken);

        if (!persisted)
        {
            _logger.LogWarning("Paymob created an intention but local persistence did not complete for transaction {TransactionId}.",
                transactionToInitialize.Id);
            return Result<PaymobPaymentIntentResult>.Failure(PaymobErrors.IntentPersistenceFailed);
        }

        return Result<PaymobPaymentIntentResult>.Success(new PaymobPaymentIntentResult(
            transactionToInitialize.Id, TransactionStatus.PaymobPending, request.Integration, request.Amount,
            _settings.Currency, created.IntentionId, created.OrderId, created.ClientSecret,
            _settings.PublicKey, false, null));
    }

    public async Task<PaymobWebhookOutcome> HandleTransactionCallbackAsync(
        JsonElement root, string? hmac, CancellationToken cancellationToken)
    {
        if (root.ValueKind != JsonValueKind.Object
            || !TryGetProperty(root, "type", out var typeElement)
            || typeElement.ValueKind != JsonValueKind.String || typeElement.GetString() != "TRANSACTION"
            || !TryGetProperty(root, "obj", out var obj) || obj.ValueKind != JsonValueKind.Object)
            return PaymobWebhookOutcome.InvalidPayload;
        if (!_callbackVerifier.IsValidTransactionHmac(obj, hmac))
            return PaymobWebhookOutcome.InvalidSignature;
        if (!TryReadCallback(obj, out var callback)) return PaymobWebhookOutcome.InvalidPayload;

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var payment = await _repository.GetPaymobPaymentForCallbackAsync(
                callback.SpecialReference, callback.OrderId, cancellationToken);
            if (payment is null) return PaymobWebhookOutcome.Ignored;
            if (await _repository.HasPaymentEventAsync(PaymentProvider.Paymob,
                    callback.ProviderEventId, cancellationToken))
                return PaymobWebhookOutcome.Duplicate;

            if (payment.ProviderIntegrationId != callback.IntegrationId
                || !string.Equals(payment.ProviderOrderId, callback.OrderId, StringComparison.Ordinal)
                || decimal.ToInt64(payment.Amount * 100m) != callback.AmountCents
                || !string.Equals(_settings.Currency, callback.Currency, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Paymob callback validation mismatch for local transaction {TransactionId}.", payment.Id);
                return PaymobWebhookOutcome.Ignored;
            }

            var previousStatus = payment.Status;
            var currentStatus = previousStatus;
            var eventType = "provider-attempt-declined";

            if (callback.IsRefunded || callback.RefundedAmountCents > 0)
            {
                eventType = "refund-requires-ledger-review";
                if (payment.MarkPaymobAdjustmentNeedsReview())
                {
                    currentStatus = payment.Status;
                    _auditRepository.Add(AuditLog.Create(payment.TenantId, null,
                        AuditAction.PaymobAdjustmentNeedsReview, payment.StudentId,
                        $"Paymob refund event for transaction {payment.Id}; manual ledger reconciliation is required."));
                }
            }
            else if (callback.IsVoided)
            {
                eventType = "provider-payment-voided";
                if (payment.MarkPaymobVoided()) currentStatus = payment.Status;
                else if (payment.MarkPaymobAdjustmentNeedsReview())
                {
                    currentStatus = payment.Status;
                    _auditRepository.Add(AuditLog.Create(payment.TenantId, null,
                        AuditAction.PaymobAdjustmentNeedsReview, payment.StudentId,
                        $"Paymob void event for transaction {payment.Id}; manual ledger reconciliation is required."));
                    eventType = "void-requires-ledger-review";
                }
            }
            else if (callback.Success && !callback.Pending)
            {
                if (callback.IsAuth && !callback.IsCapture)
                {
                    eventType = "authorization-only-awaiting-capture";
                }
                else if (payment.Status == TransactionStatus.Successful)
                {
                    eventType = "additional-success-requires-refund-review";
                    if (payment.MarkPaymobAdjustmentNeedsReview())
                    {
                        currentStatus = payment.Status;
                        _auditRepository.Add(AuditLog.Create(payment.TenantId, null,
                            AuditAction.PaymobAdjustmentNeedsReview, payment.StudentId,
                            $"Multiple successful Paymob transactions for local payment {payment.Id}; review is required."));
                    }
                }
                else
                {
                    if (payment.Status == TransactionStatus.Failed) payment.ReopenPaymobForVerifiedSuccess();
                    if (payment.Status == TransactionStatus.PaymobPending)
                    {
                        var applied = await _ledger.ApplyPaymobPaymentAsync(payment.TenantId, payment.Id, cancellationToken);
                        if (applied.IsSuccess)
                        {
                            currentStatus = TransactionStatus.Successful;
                            eventType = "payment-succeeded-and-ledger-applied";
                        }
                        else
                        {
                            payment.MarkPaymobAdjustmentNeedsReview();
                            currentStatus = payment.Status;
                            eventType = "payment-succeeded-ledger-review-required";
                            _auditRepository.Add(AuditLog.Create(payment.TenantId, null,
                                AuditAction.PaymobAdjustmentNeedsReview, payment.StudentId,
                                $"Paymob reported payment success for transaction {payment.Id}, but the financial ledger could not apply it automatically."));
                        }
                    }
                    else
                    {
                        eventType = "late-success-requires-manual-review";
                        if (payment.MarkPaymobAdjustmentNeedsReview()) currentStatus = payment.Status;
                        _auditRepository.Add(AuditLog.Create(payment.TenantId, null,
                            AuditAction.PaymobAdjustmentNeedsReview, payment.StudentId,
                            $"Late Paymob success for transaction {payment.Id}; manual review is required."));
                    }
                }
            }
            else if (callback.Pending)
            {
                eventType = "provider-attempt-pending";
            }

            var payloadHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(root.GetRawText())))
                .ToLowerInvariant();
            _repository.AddPaymentEvent(PaymentEvent.Create(payment.TenantId, payment.Id,
                PaymentProvider.Paymob, callback.ProviderEventId, eventType,
                previousStatus, currentStatus, callback.OrderId, callback.CreatedAtUtc, payloadHash));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return PaymobWebhookOutcome.Accepted;
        }, IsolationLevel.Serializable, cancellationToken);
    }

    private int GetIntegrationId(PaymobIntegrationKey key) => key switch
    {
        PaymobIntegrationKey.CaggOnlineNonShopify => GetConfiguredIntegration("CAGG_Online_NonShopify"),
        PaymobIntegrationKey.CashCashChannel => GetConfiguredIntegration("CASH_CashChannel"),
        PaymobIntegrationKey.UigOnlineNewNonShopify => GetConfiguredIntegration("UIG_OnlineNew_NonShopify"),
        PaymobIntegrationKey.VpcOnlineNonShopify => GetConfiguredIntegration("VPC_Online_NonShopify"),
        PaymobIntegrationKey.VpcShopify => GetConfiguredIntegration("VPC_Shopify"),
        _ => 0
    };

    private int GetConfiguredIntegration(string key) =>
        _settings.IntegrationIds.TryGetValue(key, out var id) ? id : 0;

    private PaymobPaymentIntentResult ToResult(Transaction transaction,
        PaymobIntegrationKey integration, bool isProcessing) => new(
        transaction.Id, transaction.Status, integration, transaction.Amount, _settings.Currency,
        transaction.ProviderIntentionId, transaction.ProviderOrderId, transaction.ProviderClientSecret,
        _settings.PublicKey, isProcessing, transaction.ProviderFailureCode);

    private static Error? ValidatePayment(FinancialPaymentOptions options, PaymentType type, decimal amount)
    {
        var otherDebts = options.Debts.Where(x => x.SessionId != options.SessionId && x.OutstandingAmount > 0).ToList();
        switch (type)
        {
            case PaymentType.DebtSettlement:
                return options.TotalOutstanding > 0 ? null : FinanceErrors.NoOutstandingDebt;
            case PaymentType.PerSession:
                if (otherDebts.Count > 0) return FinanceErrors.PreviousDebtMustBeSettled;
                return options.Debts.Any(x => x.SessionId == options.SessionId && x.OutstandingAmount > 0)
                    ? null : FinanceErrors.SessionChargeNotFound;
            case PaymentType.MonthlySubscription:
                if (otherDebts.Count > 0) return FinanceErrors.PreviousDebtMustBeSettled;
                if (options.MonthlyPrice is not { } monthlyPrice) return FinanceErrors.MonthlyPriceNotConfigured;
                if (options.HasMonthlySubscription) return FinanceErrors.SubscriptionAlreadyExists;
                return amount + options.WalletCredit >= monthlyPrice ? null : FinanceErrors.InsufficientPayment;
            default:
                return FinanceErrors.UnsupportedPaymentType;
        }
    }

    private string Fingerprint(Guid tenantId, Guid studentId, CreatePaymobPaymentRequest request, int integrationId)
    {
        var canonical = string.Join("|", tenantId.ToString("N"), studentId.ToString("N"),
            request.SessionId.ToString("N"), request.Type.ToString(),
            request.Amount.ToString("0.00", CultureInfo.InvariantCulture), integrationId.ToString(CultureInfo.InvariantCulture));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    private static string GetItemName(PaymentType type) => type switch
    {
        PaymentType.PerSession => "Session payment",
        PaymentType.MonthlySubscription => "Monthly subscription",
        PaymentType.DebtSettlement => "Debt settlement",
        _ => "TeacherERP payment"
    };

    private static bool IsPublicHttpsUrl(string value)
    {
        if (!IsHttpsUrl(value, out var uri)) return false;
        return !uri.IsLoopback && !uri.Host.EndsWith(".localhost", StringComparison.OrdinalIgnoreCase)
            && !uri.Host.EndsWith(".local", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHttpsUrl(string value) => IsHttpsUrl(value, out _);

    private static bool IsHttpsUrl(string value, out Uri uri)
    {
        if (Uri.TryCreate(value, UriKind.Absolute, out var parsed)
            && parsed.Scheme == Uri.UriSchemeHttps)
        {
            uri = parsed;
            return true;
        }
        uri = null!;
        return false;
    }

    private static bool TryReadCallback(JsonElement obj, out Callback callback)
    {
        callback = default!;
        if (!TryScalar(obj, "id", out var eventId)
            || !TryScalar(obj, "integration_id", out var integrationText)
            || !int.TryParse(integrationText, NumberStyles.None, CultureInfo.InvariantCulture, out var integrationId)
            || !TryScalar(obj, "amount_cents", out var amountText)
            || !long.TryParse(amountText, NumberStyles.None, CultureInfo.InvariantCulture, out var amountCents)
            || !TryScalar(obj, "currency", out var currency)
            || !TryBool(obj, "success", out var success)
            || !TryBool(obj, "pending", out var pending)
            || !TryBool(obj, "is_auth", out var isAuth)
            || !TryBool(obj, "is_capture", out var isCapture)
            || !TryBool(obj, "is_voided", out var isVoided)
            || !TryBool(obj, "is_refunded", out var isRefunded)
            || !TryScalar(obj, "order.id", out var orderId))
            return false;

        var specialReference = TryScalar(obj, "merchant_order_id", out var reference) ? reference : null;
        var refundedAmount = TryScalar(obj, "refunded_amount_cents", out var refunded)
            && long.TryParse(refunded, NumberStyles.None, CultureInfo.InvariantCulture, out var parsedRefunded)
            ? parsedRefunded : 0;
        DateTime? createdAt = null;
        if (TryScalar(obj, "created_at", out var created)
            && DateTimeOffset.TryParse(created, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedCreated))
            createdAt = parsedCreated.UtcDateTime;

        callback = new Callback(eventId, integrationId, amountCents, currency, orderId,
            specialReference, success, pending, isAuth, isCapture, isVoided, isRefunded,
            Math.Max(0, refundedAmount), createdAt);
        return true;
    }

    private static bool TryScalar(JsonElement root, string path, out string value)
    {
        value = string.Empty;
        var current = root;
        foreach (var part in path.Split('.'))
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(part, out current)) return false;
        }
        if (current.ValueKind == JsonValueKind.Null) return false;
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

    private static bool TryGetProperty(JsonElement root, string property, out JsonElement value)
    {
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty(property, out value))
            return true;

        value = default;
        return false;
    }

    private static bool TryBool(JsonElement root, string property, out bool value)
    {
        value = false;
        if (!root.TryGetProperty(property, out var element)
            || element.ValueKind is not (JsonValueKind.True or JsonValueKind.False)) return false;
        value = element.GetBoolean();
        return true;
    }

    private sealed record Callback(string ProviderEventId, int IntegrationId, long AmountCents,
        string Currency, string OrderId, string? SpecialReference, bool Success, bool Pending,
        bool IsAuth, bool IsCapture, bool IsVoided, bool IsRefunded, long RefundedAmountCents,
        DateTime? CreatedAtUtc);

    private static class PaymobErrors
    {
        public static readonly Error InvalidIdempotencyKey = new("Finance.InvalidIdempotencyKey", "A unique idempotency key between 8 and 128 characters is required.");
        public static readonly Error IdempotencyKeyReused = new("Finance.IdempotencyKeyReused", "The idempotency key was already used with different payment details.");
        public static readonly Error IntegrationNotConfigured = new("Finance.PaymobIntegrationNotConfigured", "The selected Paymob integration is not configured.");
        public static readonly Error ShopifyIntegrationNotSupported = new("Finance.PaymobShopifyIntegrationNotSupported", "The Shopify integration cannot be used by this API checkout flow.");
        public static readonly Error ConfigurationInvalid = new("Finance.PaymobConfigurationInvalid", "Paymob keys are missing or incomplete.");
        public static readonly Error WebhookUrlRequired = new("Finance.PaymobWebhookUrlRequired", "A public HTTPS notification URL must be configured before creating payment intentions.");
        public static readonly Error RedirectionUrlRequired = new("Finance.PaymobRedirectionUrlRequired", "A public HTTPS redirection URL must be configured before creating online payment intentions.");
        public static readonly Error RedirectionUrlInvalid = new("Finance.PaymobRedirectionUrlInvalid", "The payment redirection URL must use HTTPS.");
        public static readonly Error BillingProfileIncomplete = new("Finance.PaymobBillingProfileIncomplete", "Add an email address and phone number to the student profile before starting Paymob checkout.");
        public static readonly Error IntentCreationFailed = new("Finance.PaymobIntentCreationFailed", "Paymob could not create this payment intention.");
        public static readonly Error IntentPersistenceFailed = new("Finance.PaymobIntentPersistenceFailed", "The payment intention could not be saved. Retry with the same idempotency key.");
        public static readonly Error UnsupportedPaymentMethod = new("Finance.PaymobPaymentMethodUnsupported", "The selected Paymob integration is not supported.");
        public static readonly Error PaymentAlreadyPending = new("Finance.PaymobPaymentAlreadyPending", "A payment attempt for this item is already pending or requires review.");
    }
}
