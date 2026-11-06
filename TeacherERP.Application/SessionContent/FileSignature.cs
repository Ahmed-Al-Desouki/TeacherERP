// جديد — TeacherERP.Application/SessionContent/FileSignature.cs
namespace TeacherERP.Application.SessionContent
{
    public enum DetectedFileType { Unknown, Jpeg, Png, Webp, Pdf }

    // نوع الملف الحقيقي من أول بايتاته (لا نثق في الامتداد ولا في الـ Content-Type القادم من العميل)
    public static class FileSignature
    {
        public const int HeaderLength = 12;

        private static ReadOnlySpan<byte> PngSignature => new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        public static DetectedFileType Detect(ReadOnlySpan<byte> header)
        {
            if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            {
                return DetectedFileType.Jpeg;
            }

            if (header.Length >= 8 && header[..8].SequenceEqual(PngSignature))
            {
                return DetectedFileType.Png;
            }

            if (header.Length >= 5 && header[..5].SequenceEqual("%PDF-"u8))
            {
                return DetectedFileType.Pdf;
            }

            if (header.Length >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8))
            {
                return DetectedFileType.Webp;
            }

            return DetectedFileType.Unknown;
        }

        public static string ContentTypeOf(DetectedFileType type) => type switch
        {
            DetectedFileType.Jpeg => "image/jpeg",
            DetectedFileType.Png => "image/png",
            DetectedFileType.Webp => "image/webp",
            DetectedFileType.Pdf => "application/pdf",
            _ => "application/octet-stream"
        };

        public static string ExtensionOf(DetectedFileType type) => type switch
        {
            DetectedFileType.Jpeg => ".jpg",
            DetectedFileType.Png => ".png",
            DetectedFileType.Webp => ".webp",
            DetectedFileType.Pdf => ".pdf",
            _ => string.Empty
        };
    }
}