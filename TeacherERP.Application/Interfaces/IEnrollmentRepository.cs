// موجود (تعديل) — TeacherERP.Application/Interfaces/IEnrollmentRepository.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public record GroupJoinInfo(
        Guid GroupId, string GroupName, Guid TenantId, bool GroupIsActive, int? Capacity, TenantStatus TenantStatus, bool AutoApprove);

    public record EnrollmentRequestListItem(
        Guid Id, Guid StudentId, string StudentName, Guid GroupId, string GroupName,
        EnrollmentRequestType Type, EnrollmentRequestStatus Status, DateTime RequestedAt, string? CurrentGroupName);

    public record MyEnrollmentItem(
        Guid EnrollmentId, Guid GroupId, string GroupName, Guid TeacherId, string TeacherName, string? Subject, DateTime EnrolledAt);

    public record MyRequestItem(
        Guid RequestId, Guid GroupId, string GroupName, Guid TeacherId, EnrollmentRequestType Type,
        EnrollmentRequestStatus Status, DateTime RequestedAt, string? DecisionNote);

    // الاستعلامات اللي تأخذ tenantId صراحةً تعمل عبر المدرسين بقصد (IgnoreQueryFilters) لأن الطالب ليس عضواً بعد
    public interface IEnrollmentRepository
    {
        Task<GroupJoinInfo?> GetGroupJoinInfoAsync(Guid groupId, CancellationToken cancellationToken);
        Task<GroupEnrollment?> GetActiveEnrollmentAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken);
        Task<int> CountActiveAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken);
        Task<TenantMember?> GetMemberAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);
        Task<bool> HasPendingRequestAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken);

        void AddRequest(EnrollmentRequest request);
        void AddEnrollment(GroupEnrollment enrollment);
        void AddMember(TenantMember member);

        // للموظفين داخل سنتر المدرس (يتفلتر بالمدرس الحالي)
        Task<EnrollmentRequest?> GetTrackedRequestAsync(Guid requestId, CancellationToken cancellationToken);
        Task<List<EnrollmentRequestListItem>> GetRequestsAsync(
            EnrollmentRequestStatus? status, CancellationToken cancellationToken);

        // للطالب عبر كل مدرسيه
        Task<EnrollmentRequest?> GetOwnPendingRequestAsync(Guid requestId, Guid studentId, CancellationToken cancellationToken);
        Task<List<MyEnrollmentItem>> GetMyEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken);
        Task<List<MyRequestItem>> GetMyRequestsAsync(Guid studentId, CancellationToken cancellationToken);
    }
}