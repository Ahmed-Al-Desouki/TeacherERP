using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// يمثل حصة فعلية تم توليدها وتاريخها محدد، ترتبط بمجموعة معينة
    /// </summary>
    public class Session : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public Guid GroupId { get; private set; }
        public Group Group { get; private set; }

        /// <summary>وقت البداية والنهاية بتوقيت UTC</summary>
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        /// <summary>حالة توضح إذا تم إلغاء الحصة (لطرد ومنع حضور الطلاب)</summary>
        public bool IsCancelled { get; private set; }

        /// <summary>رابط صورة السبورة الخاصة بهذه الحصة</summary>
        public string BoardImageUrl { get; private set; }

        /// <summary>رابط صورة الإجابة النموذجية للواجبات المطلوبة في هذه الحصة</summary>
        public string HomeworkAnswerUrl { get; private set; }

        public ICollection<AttendanceRecord> Attendances { get; private set; }

        private Session() { }

        public static Session Create(Guid tenantId, Guid groupId, DateTime startTime, DateTime endTime)
        {
            return new Session
            {
                TenantId = tenantId,
                GroupId = groupId,
                StartTime = startTime,
                EndTime = endTime,
                Attendances = new List<AttendanceRecord>(),
            };
        }

        public void Cancel()
        {
            IsCancelled = true;
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