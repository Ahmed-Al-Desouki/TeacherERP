// موجود (تعديل) — TeacherERP.Infrastructure/Services/EmailTemplates.cs
using System.Net;
using TeacherERP.Application.Common;
using TeacherERP.Application.Sessions;

namespace TeacherERP.Infrastructure.Services
{
    // قوالب الإيميلات (HTML مضمّن الأنماط ليعمل مع كل برامج البريد، ثنائي اللغة)
    // الأوقات تصل بـ UTC وتُحوَّل هنا لتوقيت المستخدم عبر UserTime
    internal static class EmailTemplates
    {
        public static string VerificationOtp(string otpCode)
        {
            var code = WebUtility.HtmlEncode(otpCode);

            var content = $$"""
        <p style="margin:0 0 6px 0;">Welcome! Use the code below to verify your email. It is valid for <b>10 minutes</b>.</p>
        <p dir="rtl" style="margin:0 0 22px 0;color:#6b7280;">أهلاً بك! استخدم الكود التالي لتأكيد بريدك الإلكتروني. الكود صالح لمدة <b>10 دقائق</b>.</p>
        <div style="text-align:center;margin:0 0 22px 0;">
          <span style="display:inline-block;background:#eef2ff;color:#4338ca;font-size:34px;font-weight:700;letter-spacing:10px;padding:14px 12px 14px 22px;border-radius:12px;font-family:Consolas,'Courier New',monospace;">{{code}}</span>
        </div>
        <p style="margin:0 0 4px 0;color:#6b7280;font-size:13px;">If you didn't request this code, you can safely ignore this email.</p>
        <p dir="rtl" style="margin:0;color:#6b7280;font-size:13px;">إذا لم تطلب هذا الكود، يمكنك تجاهل هذه الرسالة بأمان.</p>
        """;

            return Layout("✉️", "#4f46e5", "Verify your email", "تأكيد البريد الإلكتروني", content);
        }

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

        public static string NewLoginAlert(string deviceName, DateTime loginAtUtc, string? timeZoneId)
        {
            var device = WebUtility.HtmlEncode(deviceName);
            var time = WebUtility.HtmlEncode(UserTime.Format(loginAtUtc, timeZoneId));

            var content = $$"""
                <p style="margin:0 0 6px 0;">A new login was detected on your account from <b>{{device}}</b> on <b>{{time}}</b>.</p>
                <p dir="rtl" style="margin:0 0 18px 0;color:#6b7280;">تم رصد تسجيل دخول جديد إلى حسابك من <b>{{device}}</b> بتاريخ <b dir="ltr">{{time}}</b>.</p>
                <div style="background:#eff6ff;border-radius:12px;padding:14px 16px;margin:0 0 18px 0;color:#1e40af;font-size:14px;">
                  If this was you, no action is needed.
                  <span dir="rtl" style="display:block;margin-top:6px;">إذا كنت أنت من سجّل الدخول فلا يلزم أي إجراء.</span>
                </div>
                <p style="margin:0 0 4px 0;color:#b91c1c;font-size:13px;"><b>Wasn't you?</b> Reset your password immediately to sign out every device.</p>
                <p dir="rtl" style="margin:0;color:#b91c1c;font-size:13px;"><b>لست أنت؟</b> أعد تعيين كلمة المرور فوراً لتسجيل الخروج من كل الأجهزة.</p>
                """;

            return Layout("🔔", "#2563eb", "New login detected", "تسجيل دخول جديد", content);
        }

        public static string PasswordChanged(DateTime changedAtUtc, string? timeZoneId)
        {
            var time = WebUtility.HtmlEncode(UserTime.Format(changedAtUtc, timeZoneId));

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

        public static string SessionEvicted(string evictedDeviceName, string newDeviceName, DateTime occurredAtUtc, string? timeZoneId)
        {
            var oldDevice = WebUtility.HtmlEncode(evictedDeviceName);
            var newDevice = WebUtility.HtmlEncode(newDeviceName);
            var time = WebUtility.HtmlEncode(UserTime.Format(occurredAtUtc, timeZoneId));
            var max = SessionPolicy.MaxActiveSessions;

            var content = $$"""
                <p style="margin:0 0 6px 0;">Your account can be signed in on up to <b>{{max}} devices</b>. A new login from <b>{{newDevice}}</b> on <b>{{time}}</b> signed you out of <b>{{oldDevice}}</b>.</p>
                <p dir="rtl" style="margin:0 0 18px 0;color:#6b7280;">يمكن تسجيل الدخول إلى حسابك على <b>{{max}} أجهزة</b> كحد أقصى. تسجيل دخول جديد من <b>{{newDevice}}</b> بتاريخ <b dir="ltr">{{time}}</b> أدى إلى تسجيل خروجك من <b>{{oldDevice}}</b>.</p>
                <div style="background:#fffbeb;border-radius:12px;padding:14px 16px;margin:0 0 18px 0;color:#92400e;font-size:14px;">
                  If this was you, no action is needed. You can log in again on that device at any time.
                  <span dir="rtl" style="display:block;margin-top:6px;">إذا كنت أنت من سجّل الدخول فلا يلزم أي إجراء، ويمكنك تسجيل الدخول على ذلك الجهاز في أي وقت.</span>
                </div>
                <p style="margin:0 0 4px 0;color:#b91c1c;font-size:13px;"><b>Wasn't you?</b> Reset your password immediately to sign out every device.</p>
                <p dir="rtl" style="margin:0;color:#b91c1c;font-size:13px;"><b>لست أنت؟</b> أعد تعيين كلمة المرور فوراً لتسجيل الخروج من كل الأجهزة.</p>
                """;

            return Layout("📱", "#d97706", "Signed out of a device", "تم تسجيل خروجك من جهاز", content);
        }

        public static string AssistantWelcome(string fullName, string teacherName, string temporaryPassword)
        {
            var name = WebUtility.HtmlEncode(fullName);
            var teacher = WebUtility.HtmlEncode(teacherName);
            var password = WebUtility.HtmlEncode(temporaryPassword);

            var content = $$"""
                <p style="margin:0 0 6px 0;">Hello <b>{{name}}</b>, <b>{{teacher}}</b> added you as an assistant on Smart Teacher ERP.</p>
                <p dir="rtl" style="margin:0 0 18px 0;color:#6b7280;">أهلاً <b>{{name}}</b>، قام <b>{{teacher}}</b> بإضافتك كمساعد على Smart Teacher ERP.</p>
                <p style="margin:0 0 6px 0;">Log in with this email and the temporary password below:</p>
                <p dir="rtl" style="margin:0 0 14px 0;color:#6b7280;">سجّل الدخول بهذا الإيميل وكلمة المرور المؤقتة التالية:</p>
                <div style="text-align:center;margin:0 0 18px 0;">
                  <span style="display:inline-block;background:#eef2ff;color:#4338ca;font-size:26px;font-weight:700;letter-spacing:3px;padding:12px 20px;border-radius:12px;font-family:Consolas,'Courier New',monospace;" dir="ltr">{{password}}</span>
                </div>
                <div style="background:#eff6ff;border-radius:12px;padding:14px 16px;margin:0 0 18px 0;color:#1e40af;font-size:14px;">
                  The first time you log in, we will send a verification code to this email. After that you can choose a new password.
                  <span dir="rtl" style="display:block;margin-top:6px;">أول مرة تسجل فيها الدخول سنرسل كود تحقق إلى هذا الإيميل، وبعدها يمكنك اختيار كلمة مرور جديدة.</span>
                </div>
                <p style="margin:0 0 4px 0;color:#b91c1c;font-size:13px;"><b>Keep this email private.</b> We recommend changing the password after you log in.</p>
                <p dir="rtl" style="margin:0;color:#b91c1c;font-size:13px;"><b>لا تشارك هذا الإيميل مع أحد.</b> ننصحك بتغيير كلمة المرور بعد تسجيل الدخول.</p>
                """;

            return Layout("👋", "#4f46e5", "Welcome to the team", "أهلاً بك في الفريق", content);
        }

        public static string AddedToTeam(string fullName, string teacherName)
        {
            var name = WebUtility.HtmlEncode(fullName);
            var teacher = WebUtility.HtmlEncode(teacherName);

            var content = $$"""
                <p style="margin:0 0 6px 0;">Hello <b>{{name}}</b>, <b>{{teacher}}</b> added you as an assistant on their team.</p>
                <p dir="rtl" style="margin:0 0 18px 0;color:#6b7280;">أهلاً <b>{{name}}</b>، قام <b>{{teacher}}</b> بإضافتك كمساعد في فريقه.</p>
                <div style="background:#eff6ff;border-radius:12px;padding:14px 16px;margin:0 0 18px 0;color:#1e40af;font-size:14px;">
                  Use your existing email and password to log in, then choose this teacher from your list.
                  <span dir="rtl" style="display:block;margin-top:6px;">استخدم نفس الإيميل وكلمة المرور الحاليين للدخول، ثم اختر هذا المدرس من قائمتك.</span>
                </div>
                <p style="margin:0 0 4px 0;color:#6b7280;font-size:13px;">If you don't know this teacher, contact the system administrator.</p>
                <p dir="rtl" style="margin:0;color:#6b7280;font-size:13px;">إذا كنت لا تعرف هذا المدرس، تواصل مع إدارة النظام.</p>
                """;

            return Layout("🤝", "#0d9488", "Added to a new team", "تمت إضافتك لفريق جديد", content);
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