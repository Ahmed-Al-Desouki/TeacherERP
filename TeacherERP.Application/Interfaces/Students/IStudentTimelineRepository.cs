using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces;

public sealed record StudentTimelineSessionItem(
    Guid SessionId,
    Guid GroupId,
    string GroupName,
    DateTime StartTime,
    DateTime EndTime,
    bool IsCancelled,
    string? CancelReason,
    AttendanceStatus? AttendanceStatus);

public interface IStudentTimelineRepository
{
    Task<PagedResult<StudentTimelineSessionItem>> GetPageAsync(Guid tenantId, Guid studentId,
        DateTime fromUtc, DateTime toUtc, Guid? groupId, PageRequest page,
        CancellationToken cancellationToken);
}
