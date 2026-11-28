using System.Text;
using System.Text.RegularExpressions;

namespace TeacherERP.Domain.Shared
{
    /// <summary>
    /// تحويل أرقام الموبايل المصرية إلى صيغة دولية موحدة (+201xxxxxxxxx).
    /// يقبل: 01012345678 / +201012345678 / 201012345678 / 00201012345678، مع الأرقام العربية والمسافات والشرطات.
    /// البادئات المقبولة: 010 و011 و012 و015.
    /// </summary>
    public static class EgyptianPhoneNumber
    {
        private static readonly Regex Pattern = new(
            @"^(?:\+20|0020|20|0)(1[0125][0-9]{8})$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool TryNormalize(string? input, out string normalized)
        {
            normalized = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var cleaned = CleanAndConvertDigits(input);
            var match = Pattern.Match(cleaned);

            if (!match.Success)
            {
                return false;
            }

            normalized = "+20" + match.Groups[1].Value;
            return true;
        }

        // حذف المسافات والشرطات والأقواس، وتحويل الأرقام العربية/الفارسية إلى أرقام لاتينية
        private static string CleanAndConvertDigits(string input)
        {
            var builder = new StringBuilder(input.Length);

            foreach (var c in input)
            {
                if (char.IsWhiteSpace(c) || c is '-' or '(' or ')')
                {
                    continue;
                }

                if (c is >= '\u0660' and <= '\u0669')
                {
                    builder.Append((char)('0' + (c - '\u0660')));
                }
                else if (c is >= '\u06F0' and <= '\u06F9')
                {
                    builder.Append((char)('0' + (c - '\u06F0')));
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }
    }
}