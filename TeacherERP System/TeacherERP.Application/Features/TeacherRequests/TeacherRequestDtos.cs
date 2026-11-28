// جديد — TeacherERP.Application/Features/TeacherRequests/TeacherRequestDtos.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.TeacherRequests
{
    // نتيجة الإرسال: الأزمنة التي يحتاجها التطبيق للعد التنازلي
    public record TeacherRequestOtpDto(string Email, int ExpiresInSeconds, int ResendAvailableInSeconds);

    // بعد تأكيد الإيميل: السر الذي يُحوَّل لـ QR (يُعرض هنا مرة واحدة فقط)
    public record TeacherRequestQrDto(string Token, DateTime ExpiresAtUtc);

    public record TeacherRequestStatusDto(TeacherAccessRequestStatus Status, DateTime ExpiresAtUtc, string? DecisionNote);

    // ما يراه المدرس بعد مسح الـ QR ليقرر
    public record TeacherRequestPreviewDto(string FullName, string Email, string Subject, string? Note, DateTime ExpiresAtUtc);
}
