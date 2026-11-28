using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Tenants
{
    // مساحة مدرس أنا عضو فيها. Role وMembershipStatus (حالتي أنا) وStatus (حالة المساحة نفسها) تظهر كنص في الـ JSON
    public record TenantDto(Guid Id, string Name, TenantRole Role, MembershipStatus MembershipStatus, TenantStatus Status)
    {
        public static TenantDto From(TenantMembershipInfo membership)
            => new(membership.TenantId, membership.TenantName, membership.Role, membership.MemberStatus, membership.TenantStatus);
    }
}