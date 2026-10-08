using System.Security.Cryptography;
using System.Text;

namespace TeacherERP.Application.Sessions
{
    // الـ Refresh Token عشوائي عالي الإنتروبيا، فيكفيه SHA-256 بدون Salt
    public static class TokenHasher
    {
        public static string Hash(string value)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }
}