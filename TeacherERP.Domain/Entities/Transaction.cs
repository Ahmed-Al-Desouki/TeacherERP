using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// يوثق كل حركة مالية داخل النظام (دفتر الأستاذ الذكي)
    /// </summary>
    public class Transaction : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public Guid StudentId { get; private set; }
        public ApplicationUser Student { get; private set; }

        /// <summary>معرف المساعد الذي قام باستلام الأموال أو تأكيد الإيصال (للمساءلة المالية)</summary>
        public Guid? ProcessedByAdminId { get; private set; }

        public decimal Amount { get; private set; }
        public PaymentType Type { get; private set; }
        public TransactionStatus Status { get; private set; }

        /// <summary>رابط صورة الإيصال المرفوعة في حالات الدفع الإلكتروني الخارجي (InstaPay)</summary>
        public string? ReceiptImageUrl { get; private set; }

        /// <summary>معرّف الإيصال في التخزين الخارجي لحذفه عند الحاجة</summary>
        public string? ReceiptImagePublicId { get; private set; }

        /// <summary>الحصة المستخدمة للتحقق عند إرسال إيصال خارجي؛ ليست بالضرورة حصةً سُدّدت عنها المعاملة</summary>
        public Guid? PaymentContextSessionId { get; private set; }

        /// <summary>يربط المعاملة المالية بحصة محددة (مهم جداً للطلاب الذين يدفعون بالحصة)</summary>
        public Guid? RelatedSessionId { get; private set; }

        /// <summary>طريقة الدفع أو مزود الدفع. المعاملات القديمة قد تتركه فارغاً.</summary>
        public PaymentProvider? Provider { get; private set; }

        /// <summary>مفتاح إعادة الطلب القادم من العميل؛ لا يُستخدم كسرّ.</summary>
        public string? IdempotencyKey { get; private set; }
        public string? RequestFingerprint { get; private set; }
        public string? ProviderIntentionId { get; private set; }
        public string? ProviderOrderId { get; private set; }
        public int? ProviderIntegrationId { get; private set; }
        /// <summary>مفتاح checkout الخاص بهذه النية. لا يُكتب في اللوجات.</summary>
        public string? ProviderClientSecret { get; private set; }
        public string? ProviderFailureCode { get; private set; }
        public DateTime? ProviderRequestStartedAtUtc { get; private set; }

        private Transaction() { }

        public static Transaction CreatePayment(Guid tenantId, Guid studentId, Guid adminId, decimal amount,
            PaymentType type, Guid? sessionId = null, string? receiptUrl = null,
            PaymentProvider paymentMethod = PaymentProvider.Cash)
        {
            return new Transaction
            {
                TenantId = tenantId,
                StudentId = studentId,
                ProcessedByAdminId = adminId,
                Amount = amount,
                Type = type,
                // إذا كان هناك إيصال، تكون الحالة معلقة حتى يوافق السوبر أدمن، وإلا تكون ناجحة فوراً
                Status = receiptUrl == null ? TransactionStatus.Successful : TransactionStatus.PendingApproval,
                RelatedSessionId = sessionId,
                ReceiptImageUrl = receiptUrl,
                Provider = paymentMethod
            };
        }

        public static Transaction CreateExternalPayment(
            Guid tenantId, Guid studentId, decimal amount, PaymentType type, Guid contextSessionId,
            string receiptUrl, string receiptPublicId)
        {
            return new Transaction
            {
                TenantId = tenantId,
                StudentId = studentId,
                Amount = amount,
                Type = type,
                Status = TransactionStatus.PendingApproval,
                RelatedSessionId = type is PaymentType.PerSession or PaymentType.MonthlySubscription ? contextSessionId : null,
                PaymentContextSessionId = contextSessionId,
                ReceiptImageUrl = receiptUrl,
                ReceiptImagePublicId = receiptPublicId,
                Provider = PaymentProvider.InstaPay
            };
        }

        public static Transaction CreatePaymobPayment(Guid tenantId, Guid studentId, decimal amount,
            PaymentType type, Guid contextSessionId, string idempotencyKey, string requestFingerprint,
            int integrationId, DateTime requestStartedAtUtc)
        {
            return new Transaction
            {
                TenantId = tenantId,
                StudentId = studentId,
                Amount = amount,
                Type = type,
                Status = TransactionStatus.PaymobPending,
                Provider = PaymentProvider.Paymob,
                PaymentContextSessionId = contextSessionId,
                RelatedSessionId = type is PaymentType.PerSession or PaymentType.MonthlySubscription ? contextSessionId : null,
                IdempotencyKey = idempotencyKey,
                RequestFingerprint = requestFingerprint,
                ProviderIntegrationId = integrationId,
                ProviderRequestStartedAtUtc = DateTime.SpecifyKind(requestStartedAtUtc, DateTimeKind.Utc)
            };
        }

        public void AttachPaymobIntention(string intentionId, string orderId, string clientSecret)
        {
            if (Provider != PaymentProvider.Paymob || Status != TransactionStatus.PaymobPending)
                throw new InvalidOperationException("Only a pending Paymob payment can receive an intention.");
            ProviderIntentionId = intentionId;
            ProviderOrderId = orderId;
            ProviderClientSecret = clientSecret;
            ProviderFailureCode = null;
            MarkAsUpdated();
        }

        public bool TryBeginPaymobInitialization(DateTime nowUtc, TimeSpan lease)
        {
            if (Provider != PaymentProvider.Paymob || Status != TransactionStatus.PaymobPending
                || !string.IsNullOrWhiteSpace(ProviderClientSecret)) return false;
            if (ProviderRequestStartedAtUtc is { } started && started > nowUtc - lease) return false;
            ProviderRequestStartedAtUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);
            MarkAsUpdated();
            return true;
        }

        public bool MarkPaymobInitializationFailed(string failureCode)
        {
            if (Provider != PaymentProvider.Paymob || Status != TransactionStatus.PaymobPending) return false;
            Status = TransactionStatus.Failed;
            ProviderClientSecret = null;
            ProviderFailureCode = string.IsNullOrWhiteSpace(failureCode) ? "Paymob.Unavailable" : failureCode[..Math.Min(failureCode.Length, 64)];
            MarkAsUpdated();
            return true;
        }

        public bool MarkPaymobSucceeded()
        {
            if (Provider != PaymentProvider.Paymob || Status != TransactionStatus.PaymobPending) return false;
            Status = TransactionStatus.Successful;
            ProviderClientSecret = null;
            ProviderFailureCode = null;
            MarkAsUpdated();
            return true;
        }

        public bool MarkPaymobVoided()
        {
            if (Provider != PaymentProvider.Paymob || Status != TransactionStatus.PaymobPending) return false;
            Status = TransactionStatus.Voided;
            PaymentContextSessionId = null;
            ProviderClientSecret = null;
            MarkAsUpdated();
            return true;
        }

        public bool MarkPaymobAdjustmentNeedsReview()
        {
            if (Provider != PaymentProvider.Paymob || Status is not (TransactionStatus.Successful or TransactionStatus.PaymobPending)) return false;
            Status = TransactionStatus.PaymobAdjustmentReview;
            ProviderClientSecret = null;
            MarkAsUpdated();
            return true;
        }

        public bool ReopenPaymobForVerifiedSuccess()
        {
            if (Provider != PaymentProvider.Paymob || Status != TransactionStatus.Failed) return false;
            Status = TransactionStatus.PaymobPending;
            MarkAsUpdated();
            return true;
        }

        public void ApproveExternalPayment(Guid reviewerId)
        {
            Status = TransactionStatus.Successful;
            ProcessedByAdminId = reviewerId;
            PaymentContextSessionId = null;
            MarkAsUpdated();
        }

        public void RejectExternalPayment(Guid reviewerId)
        {
            Status = TransactionStatus.Failed;
            ProcessedByAdminId = reviewerId;
            MarkAsUpdated();
        }
    }

    /// <summary>نوع الدفع (دفع بحصة، باشتراك شهري، أو سداد مديونية قديمة)</summary>
    public enum PaymentType { PerSession, MonthlySubscription, DebtSettlement }
    public enum TransactionStatus { PendingApproval, Successful, Failed, Voided, PaymobPending, PaymobAdjustmentReview }
    public enum PaymentProvider { Paymob, Cash, InstaPay, Other }
}
