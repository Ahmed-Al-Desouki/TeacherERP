using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword
{
    // الأزمنة تُستخدم في التطبيق للعد التنازلي وزر إعادة الإرسال
    public record ForgotPasswordResponseDto(int ExpiresInSeconds, int ResendAvailableInSeconds);
}
