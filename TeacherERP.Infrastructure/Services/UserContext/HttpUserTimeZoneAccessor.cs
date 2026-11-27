using Microsoft.AspNetCore.Http;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Sessions;

namespace TeacherERP.Infrastructure.Services
{
    // يفضّل ترويسة الطلب، ثم يرجع لمنطقة المستخدم المحفوظة في التوكن.
    public class HttpUserTimeZoneAccessor : IUserTimeZoneAccessor
    {
        public const string HeaderName = "X-Time-Zone";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpUserTimeZoneAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? TimeZoneId
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                var requestValue = context?.Request.Headers[HeaderName].ToString().Trim();
                if (IsSupported(requestValue)) return requestValue;

                var savedValue = context?.User.FindFirst(AuthClaimTypes.TimeZoneId)?.Value;
                return IsSupported(savedValue) ? savedValue : null;
            }
        }

        private static bool IsSupported(string? value)
            => string.Equals(value, "UTC", StringComparison.OrdinalIgnoreCase) || UserTime.IsValid(value);
    }
}
