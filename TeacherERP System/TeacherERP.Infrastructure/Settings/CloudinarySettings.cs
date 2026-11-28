// جديد — TeacherERP.Infrastructure/Settings/CloudinarySettings.cs
namespace TeacherERP.Infrastructure.Settings
{
    // القيم الحقيقية في appsettings.json (خارج Git) أو Environment Variables، ولا تُكتب في الكود
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }
}