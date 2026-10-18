// جديد — TeacherERP.Domain/Entities/GroupEnrollment.cs
using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// تسجيل طالب في مجموعة عند مدرس. الغياب يُحسب من EnrolledAt.
    /// الطالب له تسجيل نشط واحد فقط عند كل مدرس (Index فريد)، والنقل يُنهي القديم ويفتح الجديد.
    /// </summary>
    public class GroupEnrollment : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public Guid GroupId { get; private set; }
        public Group Group { get; private set; }

        public Guid StudentId { get; private set; }
        public ApplicationUser Student { get; private set; }

        public EnrollmentStatus Status { get; private set; }

        /// <summary>بداية التسجيل (UTC): الغياب لا يُحسب قبلها</summary>
        public DateTime EnrolledAt { get; private set; }
        public DateTime? EndedAt { get; private set; }

        /// <summary>من وافق (فارغ لو القبول تلقائي)</summary>
        public Guid? ApprovedByUserId { get; private set; }

        private GroupEnrollment() { }

        public static GroupEnrollment Create(Guid tenantId, Guid groupId, Guid studentId, DateTime enrolledAtUtc, Guid? approvedByUserId)
        {
            return new GroupEnrollment
            {
                TenantId = tenantId,
                GroupId = groupId,
                StudentId = studentId,
                Status = EnrollmentStatus.Active,
                EnrolledAt = enrolledAtUtc,
                ApprovedByUserId = approvedByUserId
            };
        }

        public void End(DateTime endedAtUtc)
        {
            Status = EnrollmentStatus.Ended;
            EndedAt = endedAtUtc;
            MarkAsUpdated();
        }
    }

    public enum EnrollmentStatus { Active, Ended }
}