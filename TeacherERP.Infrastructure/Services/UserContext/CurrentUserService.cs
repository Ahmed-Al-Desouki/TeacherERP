using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Sessions;

namespace TeacherERP.Infrastructure.Services
{
    // يقرأ هوية المستخدم والجلسة الحالية من توكن الطلب
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId => ReadGuidClaim(ClaimTypes.NameIdentifier);

        public Guid? SessionId => ReadGuidClaim(AuthClaimTypes.SessionId);

        public string? TimeZoneId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User.FindFirst(AuthClaimTypes.TimeZoneId)?.Value;
                return string.Equals(value, "UTC", StringComparison.OrdinalIgnoreCase) || UserTime.IsValid(value)
                    ? value
                    : null;
            }
        }

        private Guid? ReadGuidClaim(string claimType)
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}
