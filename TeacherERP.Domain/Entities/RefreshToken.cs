using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// كيان مسؤول عن إدارة جلسات المستخدمين (Sessions) لتوفير أمان عالي مع تقنية JWT
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public ApplicationUser User { get; private set; }

        /// <summary>سلسلة التوكن العشوائية التي يتم إرسالها للعميل (Flutter)</summary>
        public string Token { get; private set; }

        /// <summary>معرف التوكن الأساسي (JTI) المرتبط بهذا الـ Refresh Token</summary>
        public string JwtId { get; private set; }
        public DateTime ExpiryDate { get; private set; }

        /// <summary>علامة توضح إذا ما تم استخدام هذا التوكن مسبقاً (لمنع هجمات إعادة الاستخدام)</summary>
        public bool IsUsed { get; private set; }

        /// <summary>علامة توضح إبطال التوكن يدوياً بواسطة السوبر أدمن (كطرد مساعد من النظام)</summary>
        public bool IsRevoked { get; private set; }

        /// <summary>معلومات الجهاز المتصل لزيادة الرقابة الأمنية</summary>
        public string DeviceInfo { get; private set; }

        /// <summary>خاصية محسوبة (Computed Property) للتحقق من صلاحية التوكن</summary>
        public bool IsActive => !IsRevoked && !IsUsed && ExpiryDate > DateTime.UtcNow;

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, string token, string jwtId, DateTime expiryDate, string deviceInfo)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = token,
                JwtId = jwtId,
                ExpiryDate = expiryDate,
                DeviceInfo = deviceInfo
            };
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
            MarkAsUpdated();
        }
        public void Revoke()
        {
            IsRevoked = true;
            MarkAsUpdated();
        }
    }

}
