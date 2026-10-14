using Microsoft.AspNetCore.Authorization;
using TeacherERP.Domain.Entities;

namespace TeacherERP.API.Security
{
    // يحدد السنتر الحالي للطلب ويتأكد أن المستخدم عضو فيه، وبدور مسموح (بدون أدوار = أي دور)
    public class TenantRequirement : IAuthorizationRequirement
    {
        public const string HeaderName = "X-Teacher-Id";

        public IReadOnlyCollection<TenantRole> AllowedRoles { get; }

        public TenantRequirement(params TenantRole[] allowedRoles)
        {
            AllowedRoles = allowedRoles;
        }

        public bool Allows(TenantRole role) => AllowedRoles.Count == 0 || AllowedRoles.Contains(role);
    }
}