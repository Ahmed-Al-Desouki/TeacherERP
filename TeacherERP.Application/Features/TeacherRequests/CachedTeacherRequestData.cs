// جديد — TeacherERP.Application/Features/TeacherRequests/CachedTeacherRequestData.cs
namespace TeacherERP.Application.Features.TeacherRequests
{
    // بيانات طلب المدرس المعلقة في الكاش لحين تأكيد الـ OTP (لا شيء منها يصل للداتابيز قبل التأكيد)
    public sealed class CachedTeacherRequestData
    {
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public string? Note { get; init; }
        public string TimeZoneId { get; init; } = string.Empty;
    }
}
