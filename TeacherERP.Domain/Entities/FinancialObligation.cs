using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>A session charge that remains independently traceable even after the group price changes.</summary>
    public class FinancialObligation : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }
        public Guid StudentId { get; private set; }
        public Guid GroupId { get; private set; }
        public Guid SessionId { get; private set; }
        public string GroupName { get; private set; } = string.Empty;
        public DateTime SessionStartUtc { get; private set; }
        public decimal Amount { get; private set; }
        public decimal PaidAmount { get; private set; }
        public decimal OutstandingAmount => Amount - PaidAmount;
        public bool IsSettled => PaidAmount >= Amount;

        private FinancialObligation() { }

        public static FinancialObligation Create(Guid tenantId, Guid studentId, Guid groupId, Guid sessionId,
            string groupName, DateTime sessionStartUtc, decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            return new FinancialObligation
            {
                TenantId = tenantId, StudentId = studentId, GroupId = groupId,
                SessionId = sessionId, GroupName = groupName, SessionStartUtc = sessionStartUtc,
                Amount = decimal.Round(amount, 2)
            };
        }

        public decimal ApplyPayment(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            var applied = Math.Min(amount, OutstandingAmount);
            PaidAmount += applied;
            MarkAsUpdated();
            return applied;
        }

        public void AddRefundDebt(decimal amount)
        {
            if (amount <= 0 || decimal.Round(amount, 2) != amount)
                throw new ArgumentOutOfRangeException(nameof(amount));
            Amount += amount;
            MarkAsUpdated();
        }
    }
}
