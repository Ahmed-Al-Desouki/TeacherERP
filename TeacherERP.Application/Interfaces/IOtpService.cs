// موجود (تعديل) — TeacherERP.Application/Interfaces/IOtpService.cs
using System;
using System.Collections.Generic;
using System.Text;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    public interface IOtpService
    {
        Task<Result<OtpIssueResult>> GenerateAsync(OtpPurpose purpose, string identifier, CancellationToken cancellationToken);
        Task<Result<bool>> VerifyAsync(OtpPurpose purpose, string identifier, string code, CancellationToken cancellationToken);
    }

    /// <summary>الغرض من الكود، حتى لا يتداخل كود التسجيل مع كود استرجاع الباسورد أو تفعيل المساعد</summary>
    public enum OtpPurpose
    {
        Registration,
        PasswordReset,
        Activation
    }

    /// <summary>نتيجة توليد الكود، تتضمن الأزمنة التي يحتاجها التطبيق للعد التنازلي</summary>
    public record OtpIssueResult(string Code, int ExpiresInSeconds, int ResendAvailableInSeconds);

}