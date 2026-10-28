// موجود (تعديل) — TeacherERP.Domain/Entities/Session.cs
using System;
using System.Collections.Generic;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// حصة فعلية تم توليدها وتاريخها محدد، ترتبط بمجموعة معينة. هي المرجع لتسجيل الحضور.
    /// </summary>
    public class Session : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public Guid GroupId { get; private set; }
        public Group Group { get; private set; }

        /// <summary>حصة القالب التي ولّدتها، وفارغة لو الحصة إضافية أُضيفت يدوياً</summary>
        public Guid? SlotId { get; private set; }

        /// <summary>وقت البداية والنهاية بتوقيت UTC</summary>
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        /// <summary>حالة توضح إذا تم إلغاء الحصة (لطرد ومنع حضور الطلاب)</summary>
        public bool IsCancelled { get; private set; }

        /// <summary>سبب الإلغاء (يظهر للطلاب في رسالة الاعتذار)</summary>
        public string? CancelReason { get; private set; }

        /// <summary>
        /// عُدّلت يدوياً (تأجيل أو إلغاء أو إضافة). التعديل الجماعي لقالب الجدول لا يمسّ الحصة المعدّلة أبداً.
        /// </summary>
        public bool IsModified { get; private set; }

        /// <summary>متى عالج الـ Job غياب هذه الحصة (فارغ = لم تُعالَج بعد). يمنع المعالجة المزدوجة</summary>
        public DateTime? AbsencesProcessedAt { get; private set; }

        /// <summary>رابط صورة السبورة الخاصة بهذه الحصة (اختياري)</summary>
        public string? BoardImageUrl { get; private set; }

        /// <summary>رابط صورة الإجابة النموذجية للواجبات المطلوبة في هذه الحصة (اختياري)</summary>
        public string? HomeworkAnswerUrl { get; private set; }

        public ICollection<AttendanceRecord> Attendances { get; private set; }

        private Session() { }

        public static Session Create(Guid tenantId, Guid groupId, DateTime startTime, DateTime endTime, Guid? slotId = null)
        {
            return new Session
            {
                TenantId = tenantId,
                GroupId = groupId,
                SlotId = slotId,
                StartTime = startTime,
                EndTime = endTime,
                Attendances = new List<AttendanceRecord>(),
            };
        }

        /// <summary>حصة إضافية (خارج القالب) يضيفها المدرس يدوياً</summary>
        public static Session CreateExtra(Guid tenantId, Guid groupId, DateTime startTime, DateTime endTime)
        {
            var session = Create(tenantId, groupId, startTime, endTime);
            session.IsModified = true;
            return session;
        }

        /// <summary>تأجيل أو تقديم الحصة (الأوقات UTC)</summary>
        public void Reschedule(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
            IsModified = true;
            MarkAsUpdated();
        }

        /// <summary>فصل الحصة عن حصة القالب بعد حذفها من القالب (تبقى الحصة كما هي)</summary>
        public void DetachFromSlot()
        {
            SlotId = null;
            MarkAsUpdated();
        }

        /// <summary>مزامنة نهاية حصة غير معدّلة مع تغيير مدة القالب (لا تُعلَّم معدّلة يدوياً)</summary>
        public void SyncEnd(DateTime endTime)
        {
            EndTime = endTime;
            MarkAsUpdated();
        }

        public void Cancel(string? reason = null)
        {
            IsCancelled = true;
            CancelReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            IsModified = true;
            MarkAsUpdated();
        }

        /// <summary>التراجع عن الإلغاء (تبقى الحصة معدّلة يدوياً)</summary>
        public void Restore()
        {
            IsCancelled = false;
            CancelReason = null;
            MarkAsUpdated();
        }

        /// <summary>الـ Job أنهى حساب غياب هذه الحصة (ولو كانت ملغاة أو بلا غائبين)</summary>
        public void MarkAbsencesProcessed(DateTime atUtc)
        {
            AbsencesProcessedAt = atUtc;
            MarkAsUpdated();
        }

        public void SetBoardImage(string url)
        {
            BoardImageUrl = url;
            MarkAsUpdated();
        }

        public void SetHomeworkAnswer(string url)
        {
            HomeworkAnswerUrl = url;
            MarkAsUpdated();
        }
    }
}