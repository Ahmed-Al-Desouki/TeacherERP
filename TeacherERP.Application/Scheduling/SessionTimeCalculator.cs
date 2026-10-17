namespace TeacherERP.Application.Scheduling
{
    // تحويل وقت الحائط في توقيت المدرس إلى UTC (الكود الداخلي كله UTC)، بحساب التوقيت الصيفي
    public static class SessionTimeCalculator
    {
        // منطقة غير معروفة أو غير صالحة: UTC بدل رمي خطأ يوقف التوليد لكل المدرسين
        public static TimeZoneInfo ResolveZone(string? timeZoneId)
        {
            if (string.IsNullOrWhiteSpace(timeZoneId))
            {
                return TimeZoneInfo.Utc;
            }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.Utc;
            }
            catch (InvalidTimeZoneException)
            {
                return TimeZoneInfo.Utc;
            }
        }

        // أي وقت بدون Kind يُعامل كـ UTC (كل النظام UTC)
        public static DateTime NormalizeUtc(DateTime value)
            => value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);

        public static DateOnly LocalDateOf(DateTime utc, TimeZoneInfo zone)
            => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), zone));

        public static DateTime ToUtc(DateOnly date, TimeSpan localTime, TimeZoneInfo zone)
        {
            var local = date.ToDateTime(TimeOnly.FromTimeSpan(localTime), DateTimeKind.Unspecified);

            // وقت غير موجود (قفزة التوقيت الصيفي للأمام): نتقدم ساعة
            if (zone.IsInvalidTime(local))
            {
                local = local.AddHours(1);
            }

            // وقت مكرر (رجوع التوقيت الصيفي): نأخذ المرة الأولى (الأكبر في الإزاحة)
            if (zone.IsAmbiguousTime(local))
            {
                var offset = zone.GetAmbiguousTimeOffsets(local).Max();
                return DateTime.SpecifyKind(local - offset, DateTimeKind.Utc);
            }

            return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeToUtc(local, zone), DateTimeKind.Utc);
        }
    }
}