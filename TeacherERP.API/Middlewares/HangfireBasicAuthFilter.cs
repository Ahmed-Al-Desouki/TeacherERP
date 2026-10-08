using System.Security.Cryptography;
using System.Text;
using Hangfire.Dashboard;

namespace TeacherERP.API.Middlewares
{
    // حماية لوحة Hangfire بـ Basic Authentication (البيانات من الإعدادات وليس من الكود)
    public class HangfireBasicAuthFilter : IDashboardAuthorizationFilter
    {
        private readonly string _username;
        private readonly string _password;

        public HangfireBasicAuthFilter(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // إذا لم تُضبط بيانات الدخول في الإعدادات تبقى اللوحة مقفولة تماماً
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                return false;
            }

            var header = httpContext.Request.Headers.Authorization.ToString();

            if (header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(header["Basic ".Length..].Trim()));
                    var separator = decoded.IndexOf(':');

                    if (separator > 0)
                    {
                        var userMatches = IsEqual(decoded[..separator], _username);
                        var passwordMatches = IsEqual(decoded[(separator + 1)..], _password);

                        if (userMatches && passwordMatches)
                        {
                            return true;
                        }
                    }
                }
                catch (FormatException)
                {
                    // ترويسة غير صالحة: نتعامل معها كأنها بدون بيانات دخول
                }
            }

            // نطلب من المتصفح عرض نافذة إدخال اسم المستخدم والباسورد
            httpContext.Response.Headers.WWWAuthenticate = "Basic realm=\"Hangfire Dashboard\"";
            return false;
        }

        // مقارنة بزمن ثابت لمنع هجمات قياس الزمن
        private static bool IsEqual(string a, string b)
            => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
    }
}