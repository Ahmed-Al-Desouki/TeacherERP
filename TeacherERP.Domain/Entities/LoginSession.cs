using System;

namespace TeacherERP.Domain.Entities
{
    public enum SessionRevokeReason
    {
        UserLogout = 0,
        LogoutAll = 1,
        PasswordReset = 2,
        DeviceLimit = 3,
        TokenReuse = 4,
        ReplacedByNewLogin = 5,
        AdminAction = 6
    }

    /// <summary>
    /// جلسة دخول واحدة لجهاز واحد. صف واحد لكل جهاز، والـ Refresh Token يتبدّل داخل نفس الصف (Rotation)،
    /// فلا يتراكم أي عدد من الصفوف مع كل تجديد.
    /// </summary>
    public class LoginSession : BaseEntity
    {
        //public Guid UserId { get; private set; }
        public ApplicationUser User { get; private set; }

        /// <summary>معرّف ثابت للجهاز يرسله التطبيق (UUID محفوظ في التخزين الآمن)</summary>
        public string DeviceId { get; private set; }

        /// <summary>اسم الجهاز للعرض (مثل: Pixel 7 - Android 14)</summary>
        public string DeviceName { get; private set; }

        /// <summary>SHA-256 للـ Refresh Token الحالي (التوكن نفسه لا يُخزَّن أبداً)</summary>
        public string RefreshTokenHash { get; private set; }

        /// <summary>Hash التوكن السابق، لاكتشاف إعادة الاستخدام (Reuse Detection)</summary>
        public string? PreviousRefreshTokenHash { get; private set; }

        /// <summary>انتهاء الـ Refresh الحالي (يتجدد مع كل تجديد)</summary>
        public DateTime RefreshExpiresAt { get; private set; }

        /// <summary>الحد الأقصى المطلق لعمر الجلسة مهما تجدد</summary>
        public DateTime AbsoluteExpiresAt { get; private set; }

        public DateTime LastRefreshedAt { get; private set; }

        public DateTime? RevokedAt { get; private set; }
        public SessionRevokeReason? RevokeReason { get; private set; }

        private LoginSession() { }

        public static LoginSession Create(
            Guid userId,
            string deviceId,
            string deviceName,
            string refreshTokenHash,
            DateTime nowUtc,
            TimeSpan refreshLifetime,
            TimeSpan absoluteLifetime)
        {
            var absoluteExpiry = nowUtc.Add(absoluteLifetime);
            var refreshExpiry = nowUtc.Add(refreshLifetime);

            return new LoginSession
            {
                Id = userId,
                DeviceId = deviceId,
                DeviceName = deviceName,
                RefreshTokenHash = refreshTokenHash,
                RefreshExpiresAt = refreshExpiry < absoluteExpiry ? refreshExpiry : absoluteExpiry,
                AbsoluteExpiresAt = absoluteExpiry,
                LastRefreshedAt = nowUtc
            };
        }

        public bool IsActive(DateTime nowUtc)
            => RevokedAt is null && RefreshExpiresAt > nowUtc && AbsoluteExpiresAt > nowUtc;

        /// <summary>تبديل الـ Refresh Token (يُستخدم في مرحلة التجديد أ2)</summary>
        public void Rotate(string newRefreshTokenHash, DateTime nowUtc, TimeSpan refreshLifetime)
        {
            PreviousRefreshTokenHash = RefreshTokenHash;
            RefreshTokenHash = newRefreshTokenHash;

            var next = nowUtc.Add(refreshLifetime);
            RefreshExpiresAt = next < AbsoluteExpiresAt ? next : AbsoluteExpiresAt;
            LastRefreshedAt = nowUtc;
            MarkAsUpdated();
        }

        public void Revoke(SessionRevokeReason reason, DateTime nowUtc)
        {
            if (RevokedAt is not null)
            {
                return;
            }

            RevokedAt = nowUtc;
            RevokeReason = reason;
            MarkAsUpdated();
        }
    }
}