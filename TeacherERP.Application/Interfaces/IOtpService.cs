using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Application.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateAndSaveOtpAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> VerifyOtpAsync(Guid userId, string otpCode, CancellationToken cancellationToken);
    }
}
