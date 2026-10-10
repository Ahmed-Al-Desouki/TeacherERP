using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// عضوية مستخدم في سنتر. الحساب واحد، والطالب ينضم لأكتر من مدرس بعضوية لكل سنتر.
    /// </summary>
    public class TenantMember : BaseEntity
    {
        public Guid TenantId { get; private set; }
        public Tenant Tenant { get; private set; }

        public Guid UserId { get; private set; }
        public ApplicationUser User { get; private set; }

        public TenantRole Role { get; private set; }

        private TenantMember() { }

        public static TenantMember Create(Guid tenantId, Guid userId, TenantRole role)
        {
            return new TenantMember
            {
                TenantId = tenantId,
                UserId = userId,
                Role = role
            };
        }
    }

    /// <summary>الدور داخل السنتر (الطالب المسؤول ليس دوراً هنا، بل علامة على تسجيله في المجموعة)</summary>
    public enum TenantRole { Owner, Assistant, Student }
}