using Microsoft.AspNetCore.Http;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Infrastructure.Services
{
    // يقرأ المنطقة الزمنية من الـ Header الذي يرسله التطبيق مع كل طلب
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
                var value = _httpContextAccessor.HttpContext?.Request.Headers[HeaderName].ToString().Trim();
                return UserTime.IsValid(value) ? value : null;
            }
        }
    }
}