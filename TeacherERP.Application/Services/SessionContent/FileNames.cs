// جديد — TeacherERP.Application/SessionContent/FileNames.cs
namespace TeacherERP.Application.SessionContent
{
    public static class FileNames
    {
        // اسم للعرض فقط: بلا مسارات ولا رموز خطرة، والامتداد من النوع الحقيقي للملف
        public static string Sanitize(string? name, DetectedFileType type)
        {
            var raw = Path.GetFileName((name ?? string.Empty).Replace('\\', '/'));
            var stem = Path.GetFileNameWithoutExtension(raw);

            var cleaned = new string(stem
                    .Where(c => char.IsLetterOrDigit(c) || c is ' ' or '.' or '_' or '-' or '(' or ')')
                    .ToArray())
                .Trim(' ', '.');

            if (cleaned.Length == 0)
            {
                cleaned = "file";
            }

            if (cleaned.Length > 120)
            {
                cleaned = cleaned[..120];
            }

            return cleaned + FileSignature.ExtensionOf(type);
        }
    }
}