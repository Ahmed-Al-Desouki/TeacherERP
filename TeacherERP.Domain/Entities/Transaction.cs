using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// يوثق كل حركة مالية داخل النظام (دفتر الأستاذ الذكي)
    /// </summary>
    public class Transaction : BaseEntity
    {
        public Guid StudentId { get; private set; }
        public ApplicationUser Student { get; private set; }

        /// <summary>معرف المساعد الذي قام باستلام الأموال أو تأكيد الإيصال (للمساءلة المالية)</summary>
        public Guid? ProcessedByAdminId { get; private set; }

        public decimal Amount { get; private set; }
        public PaymentType Type { get; private set; }
        public TransactionStatus Status { get; private set; }

        /// <summary>رابط صورة الإيصال المرفوعة في حالات الدفع الإلكتروني الخارجي (InstaPay)</summary>
        public string ReceiptImageUrl { get; private set; }

        /// <summary>يربط المعاملة المالية بحصة محددة (مهم جداً للطلاب الذين يدفعون بالحصة)</summary>
        public Guid? RelatedSessionId { get; private set; }

        private Transaction() { }

        public static Transaction CreatePayment(Guid studentId, Guid adminId, decimal amount, PaymentType type, Guid? sessionId = null, string receiptUrl = null)
        {
            return new Transaction
            {
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

        public void ApproveExternalPayment()
        {
            Status = TransactionStatus.Successful;
            MarkAsUpdated();
        }

        public void RejectExternalPayment()
        {
            Status = TransactionStatus.Failed;
            MarkAsUpdated();
        }
    }

    /// <summary>نوع الدفع (دفع بحصة، باشتراك شهري، أو سداد مديونية قديمة)</summary>
    public enum PaymentType { PerSession, MonthlySubscription, DebtSettlement }
    public enum TransactionStatus { PendingApproval, Successful, Failed, Voided }
}
