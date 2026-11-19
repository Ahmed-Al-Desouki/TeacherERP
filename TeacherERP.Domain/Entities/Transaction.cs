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

        private Transaction() { }

        public static Transaction CreatePayment(Guid tenantId, Guid studentId, Guid adminId, decimal amount, PaymentType type, Guid? sessionId = null, string? receiptUrl = null)
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
                ReceiptImageUrl = receiptUrl
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
                ReceiptImagePublicId = receiptPublicId
            };
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
    public enum TransactionStatus { PendingApproval, Successful, Failed, Voided }
}
