using System;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// عضوية مستخدم في سنتر. الحساب واحد، والطالب ينضم لأكتر من مدرس بعضوية لكل سنتر.
    /// وكل ما يخص علاقة المستخدم بمدرس بعينه (حالته، غيابه، مديونيته، صلاحيته المالية) يعيش هنا لا على الحساب.
    /// </summary>
    public class TenantMember : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }
        public Tenant Tenant { get; private set; }

        public Guid UserId { get; private set; }
        public ApplicationUser User { get; private set; }

        public TenantRole Role { get; private set; }

        /// <summary>حالة العضوية في هذا السنتر فقط (إيقاف الغياب أو حظر المدرس)، ولا تمنع الدخول للحساب</summary>
        public MembershipStatus Status { get; private set; }

        /// <summary>عداد الغياب المتتالي عند هذا المدرس، يستخدم لقاعدة الإيقاف التلقائي</summary>
        public int ConsecutiveAbsences { get; private set; }

        /// <summary>الرصيد المالي عند هذا المدرس (الموجب رصيد متبقي، والسالب مديونية)</summary>
        public decimal CurrentBalance { get; private set; }

        /// <summary>صلاحية المساعد في استلام الأموال والاطلاع على المديونيات في هذا السنتر (للمساعدين فقط)</summary>
        public bool HasFinancialDelegation { get; private set; }

        /// <summary>المدرس (Owner) صاحب صلاحية مالية بحكم دوره، والمساعد بالتفويض فقط</summary>
        public bool CanManageFinance => Role == TenantRole.Owner || HasFinancialDelegation;

        private TenantMember() { }

        public static TenantMember Create(Guid tenantId, Guid userId, TenantRole role)
        {
            return new TenantMember
            {
                TenantId = tenantId,
                UserId = userId,
                Role = role,
                Status = MembershipStatus.Active
            };
        }

        // --- دوال التحكم في الحالة (Behavior Methods) ---

        /// <summary>منح أو سحب الصلاحية المالية (للمساعد فقط)</summary>
        public Result<bool> SetFinancialDelegation(bool granted)
        {
            if (Role != TenantRole.Assistant)
            {
                return Result<bool>.Failure(new Error(
                    "Member.DelegationNotApplicable",
                    "Financial delegation applies to assistants only."));
            }

            HasFinancialDelegation = granted;
            MarkAsUpdated();

            return Result<bool>.Success(true);
        }

        /// <summary>إضافة أموال لرصيد العضو</summary>
        public void AddBalance(decimal amount)
        {
            CurrentBalance += amount;
            MarkAsUpdated();
        }

        /// <summary>خصم أموال من رصيد العضو (يسجل كمديونية إذا تخطى الرصيد المتاح)</summary>
        public void DeductBalance(decimal amount)
        {
            CurrentBalance -= amount;
            MarkAsUpdated();
        }

        /// <summary>زيادة عداد الغياب المتتالي بخطوة واحدة</summary>
        public void IncrementAbsence()
        {
            ConsecutiveAbsences++;
            MarkAsUpdated();
        }

        /// <summary>تصفير عداد الغياب بمجرد حضور حصة واحدة</summary>
        public void ResetAbsence()
        {
            ConsecutiveAbsences = 0;
            MarkAsUpdated();
        }

        /// <summary>إيقاف العضوية في هذا السنتر (يحدث غالباً آلياً عند تخطي حد الغياب)</summary>
        public void Suspend()
        {
            Status = MembershipStatus.Suspended;
            MarkAsUpdated();
        }

        /// <summary>حظر العضو من هذا السنتر (قرار المدرس)</summary>
        public void Blacklist()
        {
            Status = MembershipStatus.Blacklisted;
            MarkAsUpdated();
        }

        /// <summary>إعادة تفعيل العضوية</summary>
        public void Activate()
        {
            Status = MembershipStatus.Active;
            MarkAsUpdated();
        }
    }

    /// <summary>الدور داخل السنتر (الطالب المسؤول ليس دوراً هنا، بل علامة على تسجيله في المجموعة)</summary>
    public enum TenantRole { Owner, Assistant, Student }

    /// <summary>حالة العضوية في سنتر واحد (مستقلة عن حالة الحساب ApplicationUser.Status)</summary>
    public enum MembershipStatus { Active, Suspended, Blacklisted }
}