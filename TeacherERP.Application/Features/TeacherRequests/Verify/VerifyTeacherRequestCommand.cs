// جديد — TeacherERP.Application/Features/TeacherRequests/Verify/VerifyTeacherRequestCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Verify
{
    // الخطوة 2: كود الإيميل. عند النجاح يُحفظ الطلب ويرجع سر الـ QR (يظهر مرة واحدة)
    public record VerifyTeacherRequestCommand(string Email, string OtpCode) : IRequest<Result<TeacherRequestQrDto>>;
}
