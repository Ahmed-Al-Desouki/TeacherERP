// موجود (تعديل) — TeacherERP.Application/Interfaces/IAttendanceRecorder.cs
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    public enum AttendanceOutcome { Recorded, PendingCompensation, PendingApproval, AlreadyRecorded }

    public record AttendanceResultDto(AttendanceOutcome Outcome, Guid? SessionId, string? GroupName, AttendanceStatus? Status);

    // طلب تسجيل حضور بعد التحقق من الكود والصلاحيات (مسؤولية المستدعي)
    public record AttendanceRequest(
        Guid TenantId,
        Guid StudentId,
        ScanMethod Method,
        double? Latitude,
        double? Longitude,
        Guid? RecordedByUserId,
        Guid? PreferredGroupId,
        bool EnforceGeofence,
        bool RegisterImmediately,
        Guid? RestrictToGroupId);

    // مصدر واحد لقواعد الحضور: اختيار الحصة، الموقع، التسجيل التلقائي، التعويض، والإشعارات
    public interface IAttendanceRecorder
    {
        Task<Result<AttendanceResultDto>> RecordAsync(AttendanceRequest request, CancellationToken cancellationToken);
    }
}