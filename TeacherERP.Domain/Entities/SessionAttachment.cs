// جديد — TeacherERP.Domain/Entities/SessionAttachment.cs
using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// ملف تابع لحصة (سبورة، شرح، واجب، إجابة نموذجية، متابعة). الملف نفسه في التخزين الخارجي، وهنا رابطه ومعرّفه.
    /// </summary>
    public class SessionAttachment : BaseEntity, ITenantOwned
    {
        public Guid TenantId { get; private set; }

        public Guid SessionId { get; private set; }
        public Session Session { get; private set; }

        public SessionAttachmentKind Kind { get; private set; }

        public string Url { get; private set; }

        /// <summary>معرّف الملف في التخزين الخارجي (للحذف)</summary>
        public string PublicId { get; private set; }

        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public long SizeBytes { get; private set; }
        public string? Caption { get; private set; }

        /// <summary>من رفعه (مدرس أو مساعد أو طالب مسؤول). بلا Foreign Key عمداً (سجل تاريخي)</summary>
        public Guid UploadedByUserId { get; private set; }

        private SessionAttachment() { }

        public static SessionAttachment Create(
            Guid tenantId,
            Guid sessionId,
            SessionAttachmentKind kind,
            string url,
            string publicId,
            string fileName,
            string contentType,
            long sizeBytes,
            string? caption,
            Guid uploadedByUserId)
        {
            return new SessionAttachment
            {
                TenantId = tenantId,
                SessionId = sessionId,
                Kind = kind,
                Url = url,
                PublicId = publicId,
                FileName = fileName,
                ContentType = contentType,
                SizeBytes = sizeBytes,
                Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim(),
                UploadedByUserId = uploadedByUserId
            };
        }
    }

    public enum SessionAttachmentKind { Board, Explanation, Homework, HomeworkAnswer, Other }
}