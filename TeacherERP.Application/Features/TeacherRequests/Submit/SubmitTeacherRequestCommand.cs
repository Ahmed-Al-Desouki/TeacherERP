// جديد — TeacherERP.Application/Features/TeacherRequests/Submit/SubmitTeacherRequestCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Submit
{
    // الخطوة 1 من صفحة الدخول: بيانات المدرس، ثم كود على إيميله. التوقيت اختياري (الافتراضي القاهرة)
    public record SubmitTeacherRequestCommand(
        string FullName,
        string Email,
        string Subject,
        string? Note = null,
        string? TimeZoneId = null) : IRequest<Result<TeacherRequestOtpDto>>;
}
