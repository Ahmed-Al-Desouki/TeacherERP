// جديد — TeacherERP.Infrastructure/Services/PasswordGenerator.cs
using System.Security.Cryptography;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Infrastructure.Services
{
    // باسورد مؤقت عشوائي 12 حرف بلا حروف ملتبسة (0/O/1/l/I)، وفيه رقمان على الأقل (سياسة الـ Identity: رقم + 6 أحرف)
    public class PasswordGenerator : IPasswordGenerator
    {
        private const int Length = 12;
        private const int MinimumDigits = 2;
        private const string Digits = "23456789";
        private const string Letters = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";

        public string Generate()
        {
            var all = Digits + Letters;
            var chars = new char[Length];

            for (var i = 0; i < Length; i++)
            {
                chars[i] = Pick(i < MinimumDigits ? Digits : all);
            }

            // خلط الأماكن حتى لا تكون الأرقام دائماً في البداية (Fisher-Yates بمصدر عشوائي آمن)
            for (var i = chars.Length - 1; i > 0; i--)
            {
                var j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars);
        }

        private static char Pick(string alphabet) => alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
    }
}