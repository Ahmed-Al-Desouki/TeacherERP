using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Centers
{
    // Role وMembershipStatus (حالتي أنا في السنتر) وStatus (حالة السنتر نفسه) تظهر كنص في الـ JSON
    public record CenterDto(Guid Id, string Name, TenantRole Role, MembershipStatus MembershipStatus, TenantStatus Status)
    {
        public static CenterDto From(TenantMembershipInfo membership)
            => new(membership.TenantId, membership.TenantName, membership.Role, membership.MemberStatus, membership.TenantStatus);
    }
}