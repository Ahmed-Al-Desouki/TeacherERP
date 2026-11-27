namespace TeacherERP.Application.Interfaces
{
    public record ResetGrant(string Token, int ExpiresInSeconds);

    // تصريح استرجاع الباسورد: يُمنح بعد التحقق من الـ OTP ويُستخدم مرة واحدة
    public interface IResetGrantService
    {
        Task<ResetGrant> IssueAsync(string email, CancellationToken cancellationToken);
        Task<bool> IsValidAsync(string email, string token, CancellationToken cancellationToken);
        Task RevokeAsync(string email, CancellationToken cancellationToken);
    }
}
