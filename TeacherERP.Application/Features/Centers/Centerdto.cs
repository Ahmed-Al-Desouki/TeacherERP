using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Centers
{
    // الـ Role والـ Status يظهران كنص في الـ JSON (JsonStringEnumConverter)
    public record CenterDto(Guid Id, string Name, TenantRole Role, TenantStatus Status)
    {
        public static CenterDto From(TenantMembershipInfo membership)
            => new(membership.TenantId, membership.TenantName, membership.Role, membership.TenantStatus);
    }
}