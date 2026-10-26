// جديد — TeacherERP.Application/Features/Users/Commands/Auth/ActivateAccount/ActivateAccountCommand.cs
using MediatR;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount
{
    // تفعيل حساب مساعد أنشأه المدرس بالـ OTP اللي اتبعت عند أول محاولة دخول، ويرجّع الجلسة مباشرة
    public record ActivateAccountCommand(string Email, string OtpCode, string DeviceId, string DeviceInfo)
        : IRequest<Result<AuthResponseDto>>;
}