// جديد — TeacherERP.Application/Features/Team/AssistantDto.cs
namespace TeacherERP.Application.Features.Team
{
    // IsActivated = false: لسه ما فعّلش بالـ OTP. IsExistingAccount = true: كان عنده حساب (مساعد عند مدرس تاني)
    public record AssistantDto(Guid UserId, string FullName, string Email, bool IsActivated, bool IsExistingAccount);
}