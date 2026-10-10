using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// سجل يوثق حضور الطالب في حصة معينة، يحتوي على آليات منع الغش والمكان وقت التسجيل
    /// </summary>
    public class AttendanceRecord : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public Guid SessionId { get; private set; }
        public Session Session { get; private set; }
        public Guid StudentId { get; private set; }
        public ApplicationUser Student { get; private set; }

        public AttendanceStatus Status { get; private set; }

        /// <summary>طريقة تسجيل الحضور للشفافية وتتبع أداء المساعدين</summary>
        public ScanMethod ScannedVia { get; private set; }

        /// <summary>من سجّل الحضور (مساعد أو طالب مسؤول)، وفارغ لو الطالب مسح لنفسه. للمساءلة والتوثيق</summary>
        public Guid? RecordedByUserId { get; private set; }

        /// <summary>موقع الطالب وقت التسجيل (لمقارنته لاحقاً في حالات النزاع)، وفارغ في المسح اليدوي</summary>
        public double? ScannedLatitude { get; private set; }
        public double? ScannedLongitude { get; private set; }

        private AttendanceRecord() { }

        /// <summary>
        /// يسجل حضور مبدئي للطالب، يمكن أن يكون (حاضر) أو (تعويض معلق) بناءً على اللوجيك في الـ Application Layer
        /// </summary>
        public static AttendanceRecord RecordPresence(
            Guid tenantId,
            Guid sessionId,
            Guid studentId,
            ScanMethod scanMethod,
            double? lat,
            double? lon,
            Guid? recordedByUserId = null,
            AttendanceStatus initialStatus = AttendanceStatus.Present)
        {
            return new AttendanceRecord
            {
                TenantId = tenantId,
                SessionId = sessionId,
                StudentId = studentId,
                Status = initialStatus, // قد تكون PendingCompensation لو الطالب في غير موعده
                ScannedVia = scanMethod,
                RecordedByUserId = recordedByUserId,
                ScannedLatitude = lat,
                ScannedLongitude = lon,
            };
        }

        /// <summary>قبول طلب التعويض المعلق واعتباره حاضراً</summary>
        public void ApproveCompensation()
        {
            Status = AttendanceStatus.Present;
            MarkAsUpdated();
        }

        /// <summary>رفض طلب التعويض المعلق</summary>
        public void RejectCompensation()
        {
            Status = AttendanceStatus.Rejected;
            MarkAsUpdated();
        }


    }

    /// <summary>حالة الحضور (حاضر، غائب، تعويض معلق في انتظار الموافقة، مرفوض)</summary>
    public enum AttendanceStatus { Present, Absent, PendingCompensation, Rejected }
    /// <summary>طريقة المسح (بواسطة الطالب لنفسه، بواسطة المساعد لكارت الطالب، أو إدخال يدوي للطوارئ)</summary>
    public enum ScanMethod { SelfScan, AdminScan, ManualEntry }
}