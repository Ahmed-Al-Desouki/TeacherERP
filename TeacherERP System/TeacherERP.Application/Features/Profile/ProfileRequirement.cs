using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Profile
{
    // قاعدة واحدة: إكمال البروفايل مطلوب من الطلاب فقط (المدرس والمساعد مستثنيان)
    public static class ProfileRequirement
    {
        public static bool AppliesTo(IEnumerable<string> roles)
            => roles.Contains(RoleNames.Student, StringComparer.OrdinalIgnoreCase);
    }
}