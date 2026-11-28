// جديد — TeacherERP.Domain/Entities/GroupAdminRequest.cs
using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// طلب مساعد لتعيين (أو إزالة) طالب مسؤول عن مجموعة، ينتظر موافقة المدرس (إلا لو فعّل الموافقة التلقائية).
    /// طلب معلّق واحد فقط لكل (مجموعة، طالب) (Index فريد).
    /// </summary>
    public class GroupAdminRequest : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public Guid GroupId { get; private set; }
        public Group Group { get; private set; }

        public Guid StudentId { get; private set; }
        public ApplicationUser Student { get; private set; }

        public GroupAdminRequestType Type { get; private set; }
        public GroupAdminRequestStatus Status { get; private set; }

        public Guid RequestedByUserId { get; private set; }

        public Guid? DecidedByUserId { get; private set; }
        public DateTime? DecidedAt { get; private set; }
        public string? DecisionNote { get; private set; }

        private GroupAdminRequest() { }

        public static GroupAdminRequest Create(
            Guid tenantId, Guid groupId, Guid studentId, GroupAdminRequestType type, Guid requestedByUserId)
        {
            return new GroupAdminRequest
            {
                TenantId = tenantId,
                GroupId = groupId,
                StudentId = studentId,
                Type = type,
                Status = GroupAdminRequestStatus.Pending,
                RequestedByUserId = requestedByUserId
            };
        }

        public void Approve(Guid decidedByUserId, DateTime atUtc)
        {
            Status = GroupAdminRequestStatus.Approved;
            DecidedByUserId = decidedByUserId;
            DecidedAt = atUtc;
            MarkAsUpdated();
        }

        public void Reject(Guid decidedByUserId, DateTime atUtc, string? note)
        {
            Status = GroupAdminRequestStatus.Rejected;
            DecidedByUserId = decidedByUserId;
            DecidedAt = atUtc;
            DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
            MarkAsUpdated();
        }
    }

    public enum GroupAdminRequestType { Appoint, Remove }
    public enum GroupAdminRequestStatus { Pending, Approved, Rejected }
}