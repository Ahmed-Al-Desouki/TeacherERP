// موجود (تعديل) — TeacherERP.Domain/Entities/EnrollmentRequest.cs
using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// طلب طالب للانضمام لمجموعة أو للانتقال لمجموعة أخرى عند نفس المدرس، ينتظر موافقة المدرس أو المساعد.
    /// طلب معلّق واحد فقط للطالب عند كل مدرس (Index فريد).
    /// </summary>
    public class EnrollmentRequest : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public Guid StudentId { get; private set; }
        public ApplicationUser Student { get; private set; }

        /// <summary>المجموعة المطلوبة</summary>
        public Guid GroupId { get; private set; }
        public Group Group { get; private set; }

        public EnrollmentRequestType Type { get; private set; }
        public EnrollmentRequestStatus Status { get; private set; }

        /// <summary>الحصة اللي الطالب مسح كودها وهو غير مسجّل (Walk-in)، فالموافقة على الطلب تسجل له حضورها. فارغة لباقي الطلبات</summary>
        public Guid? SourceSessionId { get; private set; }

        /// <summary>من قرر (فارغ لو القبول تلقائي)</summary>
        public Guid? DecidedByUserId { get; private set; }
        public DateTime? DecidedAt { get; private set; }
        public string? DecisionNote { get; private set; }

        private EnrollmentRequest() { }

        public static EnrollmentRequest Create(
            Guid tenantId, Guid studentId, Guid groupId, EnrollmentRequestType type, Guid? sourceSessionId = null)
        {
            return new EnrollmentRequest
            {
                TenantId = tenantId,
                StudentId = studentId,
                GroupId = groupId,
                Type = type,
                Status = EnrollmentRequestStatus.Pending,
                SourceSessionId = sourceSessionId
            };
        }

        public void Approve(Guid? decidedByUserId, DateTime atUtc)
        {
            Status = EnrollmentRequestStatus.Approved;
            DecidedByUserId = decidedByUserId;
            DecidedAt = atUtc;
            MarkAsUpdated();
        }

        public void Reject(Guid decidedByUserId, DateTime atUtc, string? note)
        {
            Status = EnrollmentRequestStatus.Rejected;
            DecidedByUserId = decidedByUserId;
            DecidedAt = atUtc;
            DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
            MarkAsUpdated();
        }

        public void Cancel(DateTime atUtc)
        {
            Status = EnrollmentRequestStatus.Cancelled;
            DecidedAt = atUtc;
            MarkAsUpdated();
        }
    }

    public enum EnrollmentRequestType { Join, Transfer }
    public enum EnrollmentRequestStatus { Pending, Approved, Rejected, Cancelled }
}