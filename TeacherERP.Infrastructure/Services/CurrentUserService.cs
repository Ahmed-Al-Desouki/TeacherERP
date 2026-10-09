using System.Security.Claims;
using Microsoft.AspNetCore.Http;
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

        private Guid? ReadGuidClaim(string claimType)
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}