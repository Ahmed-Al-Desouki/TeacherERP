namespace TeacherERP.Application.Common
{
    // تحويل وقت UTC إلى التوقيت المحلي للمستخدم وتنسيقه للعرض (الإيميلات والإشعارات).
    // الكود الداخلي كله يبقى UTC، والتحويل يحدث عند الحافة فقط.
    public static class UserTime
    {
        /// <summary>هل المعرّف اسم منطقة زمنية صالح (IANA مثل Africa/Cairo)؟</summary>
        public static bool IsValid(string? timeZoneId) => TryGetTimeZone(timeZoneId, out _);

        /// <summary>
        /// مثال: 2026-10-09 17:30 (Cairo, UTC+3).
        /// إذا كانت المنطقة غير معروفة أو غير صالحة: 2026-10-09 14:30 UTC
        /// </summary>
        public static string Format(DateTime utc, string? timeZoneId)
        {
            var utcTime = ToUtc(utc);

            if (!TryGetTimeZone(timeZoneId, out var zone))
            {
                return $"{utcTime:yyyy-MM-dd HH:mm} UTC";
            }

            var local = TimeZoneInfo.ConvertTimeFromUtc(utcTime, zone);
            var offset = zone.GetUtcOffset(utcTime);
            var city = timeZoneId![(timeZoneId.LastIndexOf('/') + 1)..].Replace('_', ' ');

            return $"{local:yyyy-MM-dd HH:mm} ({city}, {FormatOffset(offset)})";
        }

        private static bool TryGetTimeZone(string? timeZoneId, out TimeZoneInfo zone)
        {
            zone = TimeZoneInfo.Utc;

            if (string.IsNullOrWhiteSpace(timeZoneId)
                || string.Equals(timeZoneId, "UTC", StringComparison.OrdinalIgnoreCase)
                || !HasValidShape(timeZoneId))
            {
                return false;
            }

            try
            {
                zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return true;
            }
            catch (TimeZoneNotFoundException)
            {
                return false;
            }
            catch (InvalidTimeZoneException)
            {
                return false;
            }
        }

        // حروف وأرقام و _ + - / فقط، بطول معقول (حماية من القيم الغريبة في الـ Header)
        private static bool HasValidShape(string value)
            => value.Length <= 64 && value.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '+' or '-' or '/');

        private static DateTime ToUtc(DateTime value) => value.Kind == DateTimeKind.Local
            ? value.ToUniversalTime()
            : DateTime.SpecifyKind(value, DateTimeKind.Utc);

        private static string FormatOffset(TimeSpan offset)
        {
            var sign = offset < TimeSpan.Zero ? "-" : "+";
            var abs = offset.Duration();

            return abs.Minutes == 0
                ? $"UTC{sign}{abs.Hours}"
                : $"UTC{sign}{abs.Hours}:{abs.Minutes:00}";
        }
    }
}