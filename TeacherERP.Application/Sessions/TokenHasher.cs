using System.Security.Cryptography;
using System.Text;

namespace TeacherERP.Application.Sessions
{
    // الـ Refresh Token عشوائي عالي الإنتروبيا، فيكفيه SHA-256 بدون Salt
    public static class TokenHasher
    {
        public static string Hash(string value)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

        // مقارنة بزمن ثابت لمنع هجمات قياس الزمن
        public static bool Matches(string hashA, string hashB)
            => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(hashA), Encoding.UTF8.GetBytes(hashB));
    }
}