namespace TeacherERP.Application.Features.Users.Commands.Auth.Login
{
    // RequiresProfileCompletion: true فقط لطالب بروفايله ناقص (يوجّه التطبيق لشاشة الإكمال)
    public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiryDate, bool RequiresProfileCompletion);
}