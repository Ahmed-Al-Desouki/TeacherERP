using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>طلب مدرس للانضمام إلى سنتر. غير معزول بالـ Tenant لأن مالك السنتر يراجعه عبر مساحة مختلفة.</summary>
    public class CenterJoinRequest : BaseEntity
    {
        public Guid CenterId { get; private set; }
        public Guid TenantId { get; private set; }
        public CenterJoinRequestStatus Status { get; private set; }
        public DateTime LastNotifiedAtUtc { get; private set; }
        public Guid? ReviewedByUserId { get; private set; }
        public DateTime? ReviewedAtUtc { get; private set; }
        public string? DecisionNote { get; private set; }

        private CenterJoinRequest() { }

        public static CenterJoinRequest Create(Guid centerId, Guid tenantId, DateTime nowUtc)
            => new()
            {
                CenterId = centerId,
                TenantId = tenantId,
                Status = CenterJoinRequestStatus.Pending,
                LastNotifiedAtUtc = nowUtc
            };

        public bool CanResendAt(DateTime nowUtc, TimeSpan resendInterval)
            => Status == CenterJoinRequestStatus.Pending && LastNotifiedAtUtc.Add(resendInterval) <= nowUtc;

        public void MarkOwnerNotified(DateTime nowUtc)
        {
            LastNotifiedAtUtc = nowUtc;
            MarkAsUpdated();
        }

        public void Approve(Guid ownerUserId, DateTime nowUtc)
        {
            Status = CenterJoinRequestStatus.Approved;
            ReviewedByUserId = ownerUserId;
            ReviewedAtUtc = nowUtc;
            MarkAsUpdated();
        }

        public void Reject(Guid ownerUserId, DateTime nowUtc, string? reason)
        {
            Status = CenterJoinRequestStatus.Rejected;
            ReviewedByUserId = ownerUserId;
            ReviewedAtUtc = nowUtc;
            DecisionNote = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            MarkAsUpdated();
        }

        public void Cancel(DateTime nowUtc)
        {
            Status = CenterJoinRequestStatus.Cancelled;
            ReviewedAtUtc = nowUtc;
            MarkAsUpdated();
        }
    }

    public enum CenterJoinRequestStatus { Pending, Approved, Rejected, Cancelled }
}
