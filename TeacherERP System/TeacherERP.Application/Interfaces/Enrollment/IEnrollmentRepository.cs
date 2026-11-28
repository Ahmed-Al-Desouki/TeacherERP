// موجود (تعديل) — TeacherERP.Application/Interfaces/IEnrollmentRepository.cs
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Application.Interfaces
{
    public record GroupJoinInfo(
        Guid GroupId, string GroupName, Guid TenantId, bool GroupIsActive, int? Capacity,
        EducationStage? EducationStage, int? GradeNumber, TenantStatus TenantStatus, bool AutoApprove);

    public record EnrollmentRequestListItem(
        Guid Id, Guid StudentId, string StudentName, string? ProfileImageUrl, Guid GroupId, string GroupName,
        EnrollmentRequestType Type, EnrollmentRequestStatus Status, DateTime RequestedAt, string? CurrentGroupName);

    public record MyEnrollmentItem(
        Guid EnrollmentId, Guid GroupId, string GroupName, Guid TeacherId, string TeacherName, string? Subject, DateTime EnrolledAt);

    public record MyRequestItem(
        Guid RequestId, Guid GroupId, string GroupName, Guid TeacherId, string? ProfileImageUrl, EnrollmentRequestType Type,
        EnrollmentRequestStatus Status, DateTime RequestedAt, string? DecisionNote);

    // الاستعلامات اللي تأخذ tenantId صراحةً تعمل عبر المدرسين بقصد (IgnoreQueryFilters) لأن الطالب ليس عضواً بعد
    public interface IEnrollmentRepository
    {
        Task<GroupJoinInfo?> GetGroupJoinInfoAsync(Guid groupId, CancellationToken cancellationToken);
        Task<GroupEnrollment?> GetActiveEnrollmentAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken);
        Task<int> CountActiveAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken);
        Task<TenantMember?> GetMemberAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);
        Task<bool> HasPendingRequestAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken);
        Task<EnrollmentRequest?> GetPendingRequestAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken);

        void AddRequest(EnrollmentRequest request);
        void AddEnrollment(GroupEnrollment enrollment);
        void AddMember(TenantMember member);

        // للموظفين داخل سنتر المدرس (يتفلتر بالمدرس الحالي)
        Task<EnrollmentRequest?> GetTrackedRequestAsync(Guid requestId, CancellationToken cancellationToken);

        // الأحدث أولاً، بصفحات
        Task<PagedResult<EnrollmentRequestListItem>> GetRequestsPageAsync(
            EnrollmentRequestStatus? status, PageRequest page, CancellationToken cancellationToken);

        // للطالب عبر كل مدرسيه
        Task<EnrollmentRequest?> GetOwnPendingRequestAsync(Guid requestId, Guid studentId, CancellationToken cancellationToken);
        Task<List<MyEnrollmentItem>> GetMyEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken);

        // طلباتي بصفحات، الأحدث أولاً
        Task<PagedResult<MyRequestItem>> GetMyRequestsPageAsync(Guid studentId, PageRequest page, CancellationToken cancellationToken);
    }
}
