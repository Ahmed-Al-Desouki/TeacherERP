using System.Net;

namespace TeacherERP.Infrastructure.Services
{
    // قوالب الإيميلات (HTML مضمّن الأنماط ليعمل مع كل برامج البريد، ثنائي اللغة)
    internal static class EmailTemplates
    {
        public static string PasswordResetOtp(string otpCode)
        {
            var code = WebUtility.HtmlEncode(otpCode);

            var content = $$"""
                <p style="margin:0 0 6px 0;">Use the code below to reset your password. It is valid for <b>10 minutes</b>.</p>
                <p dir="rtl" style="margin:0 0 22px 0;color:#6b7280;">استخدم الكود التالي لاسترجاع كلمة المرور. الكود صالح لمدة <b>10 دقائق</b>.</p>
                <div style="text-align:center;margin:0 0 22px 0;">
                  <span style="display:inline-block;background:#eef2ff;color:#4338ca;font-size:34px;font-weight:700;letter-spacing:10px;padding:14px 12px 14px 22px;border-radius:12px;font-family:Consolas,'Courier New',monospace;">{{code}}</span>
                </div>
                <p style="margin:0 0 4px 0;color:#6b7280;font-size:13px;">If you didn't request this, you can safely ignore this email.</p>
                <p dir="rtl" style="margin:0;color:#6b7280;font-size:13px;">إذا لم تطلب ذلك، يمكنك تجاهل هذه الرسالة بأمان.</p>
                """;

            return Layout("🔐", "#4f46e5", "Reset your password", "استرجاع كلمة المرور", content);
        }

        public static string PasswordChanged(DateTime changedAtUtc)
        {
            var time = WebUtility.HtmlEncode($"{changedAtUtc:yyyy-MM-dd HH:mm} UTC");

            var content = $$"""
                <p style="margin:0 0 6px 0;">Your password was changed successfully on <b>{{time}}</b>.</p>
                <p dir="rtl" style="margin:0 0 18px 0;color:#6b7280;">تم تغيير كلمة المرور بنجاح بتاريخ <b dir="ltr">{{time}}</b>.</p>
                <div style="background:#ecfdf5;border-radius:12px;padding:14px 16px;margin:0 0 18px 0;color:#065f46;font-size:14px;">
                  For your security, you were signed out of all devices. Please log in again with your new password.
                  <span dir="rtl" style="display:block;margin-top:6px;">حفاظاً على أمانك، تم تسجيل خروجك من جميع الأجهزة. يرجى تسجيل الدخول مرة أخرى بكلمة المرور الجديدة.</span>
                </div>
                <p style="margin:0 0 4px 0;color:#b91c1c;font-size:13px;"><b>Wasn't you?</b> Reset your password immediately and contact your center administrator.</p>
                <p dir="rtl" style="margin:0;color:#b91c1c;font-size:13px;"><b>لست أنت؟</b> أعد تعيين كلمة المرور فوراً وتواصل مع إدارة السنتر.</p>
                """;

            return Layout("✅", "#16a34a", "Password changed", "تم تغيير كلمة المرور", content);
        }

        private static string Layout(string icon, string accent, string title, string titleAr, string content)
        {
            return $$"""
                <!DOCTYPE html>
                <html>
                <body style="margin:0;padding:0;background:#f3f4f8;font-family:'Segoe UI',Tahoma,Arial,sans-serif;">
                  <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background:#f3f4f8;padding:32px 12px;">
                    <tr><td align="center">
                      <table role="presentation" width="520" cellpadding="0" cellspacing="0" style="max-width:520px;width:100%;background:#ffffff;border-radius:16px;overflow:hidden;">
                        <tr><td align="center" style="background:{{accent}};padding:32px 24px;">
                          <div style="font-size:44px;line-height:1;">{{icon}}</div>
                          <div style="color:#ffffff;font-size:22px;font-weight:700;margin-top:12px;">{{title}}</div>
                          <div dir="rtl" style="color:#ffffff;opacity:0.9;font-size:16px;margin-top:4px;">{{titleAr}}</div>
                        </td></tr>
                        <tr><td style="padding:28px 28px 24px 28px;color:#374151;font-size:15px;line-height:1.7;">
                          {{content}}
                        </td></tr>
                        <tr><td align="center" style="padding:16px 28px 24px 28px;border-top:1px solid #eef0f4;color:#9ca3af;font-size:12px;">
                          Smart Teacher ERP &middot; Automated message, please do not reply.<br/>
                          <span dir="rtl">رسالة آلية، يرجى عدم الرد عليها.</span>
                        </td></tr>
                      </table>
                    </td></tr>
                  </table>
                </body>
                </html>
                """;
        }
    }
}