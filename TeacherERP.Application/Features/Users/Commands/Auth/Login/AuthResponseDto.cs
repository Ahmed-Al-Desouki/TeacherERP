// موجود (تعديل) — TeacherERP.Application/Features/Users/Commands/Auth/Login/AuthResponseDto.cs
namespace TeacherERP.Application.Features.Users.Commands.Auth.Login
{
    // RequiresProfileCompletion: true فقط لطالب بروفايله ناقص (يوجّه التطبيق لشاشة الإكمال)
    // RequiresPasswordSetup: true فقط بعد تفعيل حساب مساعد لأول مرة (التطبيق يعرض "تغيير الباسورد؟" مع Skip)
    public record AuthResponseDto(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiryDate,
        bool RequiresProfileCompletion,
        bool RequiresPasswordSetup = false);
}