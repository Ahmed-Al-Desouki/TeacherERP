// جديد — TeacherERP.Application/Attendance/AttendanceCodeService.cs
using System.Security.Cryptography;
using System.Text;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Attendance
{
    public class AttendanceCodeService : IAttendanceCodeService
    {
        public string GenerateDynamicCode(Guid tenantId, string secret, DateTime atUtc)
            => Compute(tenantId, secret, WindowOf(atUtc));

        public int SecondsRemaining(DateTime atUtc)
            => AttendancePolicy.DynamicCodeWindowSeconds - (int)(ToUnixSeconds(atUtc) % AttendancePolicy.DynamicCodeWindowSeconds);

        public bool IsValid(AttendanceMode mode, Guid tenantId, string secret, string code, DateTime nowUtc)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var presented = code.Trim();

            return mode switch
            {
                AttendanceMode.StaticGlobalQR => FixedEquals(secret, presented.ToLowerInvariant()),
                AttendanceMode.DynamicScreenQR => IsCurrentOrPreviousWindow(tenantId, secret, presented.ToUpperInvariant(), nowUtc),
                _ => false
            };
        }

        private static bool IsCurrentOrPreviousWindow(Guid tenantId, string secret, string code, DateTime nowUtc)
        {
            var window = WindowOf(nowUtc);

            // نفحص الاثنين دائماً (بدون Short-circuit) لتقليل الفروق الزمنية
            var current = FixedEquals(Compute(tenantId, secret, window), code);
            var previous = FixedEquals(Compute(tenantId, secret, window - 1), code);

            return current | previous;
        }

        // مفتاح الكود المتغير مشتق من السر بـ HMAC ووسم ثابت، فالكود الثابت المطبوع (وهو السر نفسه) لا يكفي لتوليد الأكواد المتغيرة
        private const string DynamicKeyLabel = "attendance-dynamic-qr-v1";

        private static string Compute(Guid tenantId, string secret, long window)
        {
            var dynamicKey = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(secret),
                Encoding.UTF8.GetBytes(DynamicKeyLabel));

            var hash = HMACSHA256.HashData(
                dynamicKey,
                Encoding.UTF8.GetBytes($"{tenantId:N}|{window}"));

            return Convert.ToHexString(hash)[..8];
        }

        private static long WindowOf(DateTime utc) => ToUnixSeconds(utc) / AttendancePolicy.DynamicCodeWindowSeconds;

        private static long ToUnixSeconds(DateTime utc)
            => new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc)).ToUnixTimeSeconds();

        // مقارنة بزمن ثابت لمنع هجمات قياس الزمن
        private static bool FixedEquals(string expected, string actual)
            => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(actual));
    }
}