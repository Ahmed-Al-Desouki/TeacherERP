using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>دعوة خاصة أحادية الاستخدام؛ يُحفظ hash الرمز فقط، ولا ترتبط بمساحة مدرس حتى يقبلها.</summary>
    public class CenterInvitation : BaseEntity
    {
        public Guid CenterId { get; private set; }
        public Guid CreatedByUserId { get; private set; }
        public string TokenHash { get; private set; } = string.Empty;
        public CenterInvitationStatus Status { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public Guid? AcceptedByTenantId { get; private set; }
        public DateTime? AcceptedAtUtc { get; private set; }

        private CenterInvitation() { }

        public static CenterInvitation Create(Guid centerId, Guid createdByUserId, string tokenHash, DateTime expiresAtUtc)
            => new()
            {
                CenterId = centerId,
                CreatedByUserId = createdByUserId,
                TokenHash = tokenHash,
                Status = CenterInvitationStatus.Pending,
                ExpiresAtUtc = expiresAtUtc
            };

        public bool IsExpired(DateTime nowUtc)
            => Status == CenterInvitationStatus.Pending && ExpiresAtUtc <= nowUtc;

        public void Accept(Guid tenantId, DateTime nowUtc)
        {
            Status = CenterInvitationStatus.Accepted;
            AcceptedByTenantId = tenantId;
            AcceptedAtUtc = nowUtc;
            MarkAsUpdated();
        }

        public void Expire()
        {
            Status = CenterInvitationStatus.Expired;
            MarkAsUpdated();
        }
    }

    public enum CenterInvitationStatus { Pending, Accepted, Expired }
}
