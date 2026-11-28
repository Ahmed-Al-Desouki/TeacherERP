// جديد — TeacherERP.Application/Features/TeacherRequests/TeacherRequestToken.cs
using System.Security.Cryptography;
using System.Text;

namespace TeacherERP.Application.Features.TeacherRequests
{
    // سر الـ QR: 32 بايت عشوائية آمنة (Base64Url). يُعرض مرة واحدة لصاحب الطلب، ونخزّن بصمته فقط
    public static class TeacherRequestToken
    {
        public static string Generate() => System.Buffers.Text.Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));

        public static string Hash(string token)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token.Trim()))).ToLowerInvariant();
    }
}
