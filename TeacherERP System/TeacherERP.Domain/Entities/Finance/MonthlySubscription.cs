using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>Calendar-month coverage for one student in one group.</summary>
    public class MonthlySubscription : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }
        public Guid StudentId { get; private set; }
        public Guid GroupId { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public decimal Amount { get; private set; }
        public Guid PaymentTransactionId { get; private set; }

        private MonthlySubscription() { }

        public static MonthlySubscription Create(Guid tenantId, Guid studentId, Guid groupId, int year, int month, decimal amount, Guid paymentTransactionId)
        {
            if (year < 1 || month is < 1 or > 12) throw new ArgumentOutOfRangeException(nameof(month));
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            return new MonthlySubscription
            {
                TenantId = tenantId, StudentId = studentId, GroupId = groupId,
                Year = year, Month = month, Amount = decimal.Round(amount, 2),
                PaymentTransactionId = paymentTransactionId
            };
        }
    }
}
