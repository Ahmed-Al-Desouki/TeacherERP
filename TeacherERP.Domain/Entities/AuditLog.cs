// جديد — TeacherERP.Domain/Entities/AuditLog.cs
using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// سجل تدقيق للإجراءات الحساسة داخل مساحة المدرس (إيقاف وفك إيقاف وحظر الطلاب). للإضافة فقط ولا يُعدَّل.
    /// المنفّذ والهدف بلا Foreign Keys عمداً: سجل تاريخي لا يتأثر بحذف حساب لاحقاً.
    /// </summary>
    public class AuditLog : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        /// <summary>من نفّذ الإجراء، وفارغ لو النظام (إيقاف تلقائي)</summary>
        public Guid? ActorUserId { get; private set; }

        public AuditAction Action { get; private set; }

        /// <summary>الطالب (أو المستخدم) المتأثر بالإجراء</summary>
        public Guid? TargetUserId { get; private set; }

        public string? Reason { get; private set; }

        private AuditLog() { }

        public static AuditLog Create(Guid tenantId, Guid? actorUserId, AuditAction action, Guid? targetUserId, string? reason)
        {
            return new AuditLog
            {
                TenantId = tenantId,
                ActorUserId = actorUserId,
                Action = action,
                TargetUserId = targetUserId,
                Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim()
            };
        }
    }

    public enum AuditAction
    {
        StudentAutoSuspended,
        StudentUnsuspended,
        StudentReinstatedByCompensation,
        StudentBlacklisted,
        StudentUnblacklisted
    }
}