using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Centers
{
    public record CenterJoinRequestListItem(
        Guid Id,
        Guid CenterId,
        string CenterName,
        Guid TeacherTenantId,
        string TeacherName,
        string? TeacherEmail,
        string? Subject,
        CenterJoinRequestStatus Status,
        DateTime CreatedAtUtc,
        DateTime LastNotifiedAtUtc,
        DateTime? ReviewedAtUtc,
        string? DecisionNote);

    public record CenterInvitationDto(string Token, DateTime ExpiresAtUtc);
}
