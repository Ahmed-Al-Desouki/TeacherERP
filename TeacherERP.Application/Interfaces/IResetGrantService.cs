namespace TeacherERP.Application.Interfaces
{
    public record ResetGrant(string Token, int ExpiresInSeconds);

    // تصريح استرجاع الباسورد: يُمنح بعد التحقق من الـ OTP ويُستخدم مرة واحدة
    public interface IResetGrantService
    {
        ResetGrant Issue(string email);
        bool IsValid(string email, string token);
        void Revoke(string email);
    }
}