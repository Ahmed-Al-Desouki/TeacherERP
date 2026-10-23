// جديد — TeacherERP.Application/Interfaces/IAttendanceRepository.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public record AttendanceSettingsInfo(
        Guid TenantId, TenantStatus TenantStatus, AttendanceMode Mode, string QrSecret, bool RejectMockLocations, bool AutoRegisterOnScan);

    public record RunningSessionInfo(
        Guid SessionId, Guid GroupId, string GroupName, DateTime StartTime, DateTime EndTime,
        double? Latitude, double? Longitude, int GeofenceRadiusMeters);

    public record AttendanceListItem(
        Guid RecordId, Guid StudentId, string StudentName, AttendanceStatus Status, ScanMethod ScannedVia,
        DateTime RecordedAt, Guid? CompensatesSessionId);

    public record PendingCompensationItem(
        Guid RecordId, Guid StudentId, string StudentName, Guid SessionId, string GroupName, DateTime SessionStart,
        Guid? CompensatesSessionId, DateTime? CompensatesSessionStart, string? CompensatesGroupName, DateTime RecordedAt);

    // الاستعلامات اللي تأخذ tenantId صراحةً تعمل عبر المدرسين بقصد لأن الطالب ليس عضواً بعد
    public interface IAttendanceRepository
    {
        Task<AttendanceSettingsInfo?> GetSettingsAsync(Guid tenantId, CancellationToken cancellationToken);

        // إعدادات المدرس الحالي متتبَّعة للتعديل (يتفلتر بالمدرس الحالي)
        Task<TenantSettings?> GetTrackedSettingsAsync(CancellationToken cancellationToken);

        // الحصص الجارية الآن (من قبل البداية بدقائق الدخول المبكر حتى النهاية). assistantUserId يقصرها على مجموعاته
        Task<List<RunningSessionInfo>> GetRunningSessionsAsync(
            Guid tenantId, DateTime nowUtc, int earlyMinutes, Guid? assistantUserId, CancellationToken cancellationToken);

        Task<AttendanceRecord?> GetRecordAsync(Guid sessionId, Guid studentId, CancellationToken cancellationToken);

        // أقرب حصة لمجموعة الطالب الأصلية لم يحضرها (ماضية أو قادمة داخل النافذة)، ليعوّضها
        Task<Guid?> FindCompensableSessionAsync(
            Guid tenantId, Guid groupId, Guid studentId, DateTime nowUtc, int windowDays, CancellationToken cancellationToken);

        Task<Guid?> GetStudentIdByQrAsync(string studentQrCode, CancellationToken cancellationToken);

        void AddRecord(AttendanceRecord record);

        // للموظفين داخل المدرس الحالي
        Task<AttendanceRecord?> GetTrackedRecordAsync(Guid recordId, CancellationToken cancellationToken);
        Task<bool> IsAssistantAssignedToSessionAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken);
        Task<List<AttendanceListItem>> GetSessionAttendanceAsync(Guid sessionId, CancellationToken cancellationToken);
        Task<List<PendingCompensationItem>> GetPendingCompensationsAsync(Guid userId, bool seesAll, CancellationToken cancellationToken);
    }
}