using Microsoft.AspNetCore.Authorization;

namespace TeacherERP.API.Security
{
    // يحدد السنتر الحالي للطلب ويتأكد أن المستخدم عضو فيه
    public class TenantRequirement : IAuthorizationRequirement
    {
        public const string HeaderName = "X-Center-Id";
    }
}