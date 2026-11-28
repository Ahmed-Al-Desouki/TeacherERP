using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Administration
{
    // IsActivated = الإيميل اتأكد بالـ OTP (المدرس لسه ما سجّلش دخوله لو false)
    public record CreatedTeacherDto(Guid UserId, Guid TenantId, string FullName, string Email, bool IsActivated);

    public record AdminCenterDto(Guid Id, string Name, Guid OwnerUserId);

    public record CenterTeacherLinkDto(Guid CenterId, Guid TenantId, Guid TeacherUserId, CenterTeacherStatus Status);
}
