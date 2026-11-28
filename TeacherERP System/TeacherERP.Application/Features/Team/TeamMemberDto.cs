// جديد — TeacherERP.Application/Features/Team/TeamMemberDto.cs
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Team
{
    // مساعد في فريقي. Status (Active/Suspended) تظهر كنص، وIsActivated=false يعني لسه ما فعّلش بالـ OTP
    public record TeamMemberDto(
        Guid UserId,
        string FullName,
        string Email,
        MembershipStatus Status,
        bool HasFinancialDelegation,
        bool IsActivated,
        DateTime JoinedAt)
    {
        public static TeamMemberDto From(TeamMemberItem item)
            => new(item.UserId, item.FullName, item.Email, item.Status, item.HasFinancialDelegation, item.IsActivated, item.JoinedAt);
    }
}