// جديد — TeacherERP.Application/Interfaces/IGroupAdminRepository.cs
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public record GroupAdminItem(Guid StudentId, string FullName, string? ProfileImageUrl, DateTime? Since);

    public record GroupAdminRequestItem(
        Guid Id,
        Guid GroupId,
        string GroupName,
        Guid StudentId,
        string StudentName,
        GroupAdminRequestType Type,
        GroupAdminRequestStatus Status,
        Guid RequestedByUserId,
        string? RequestedByName,
        DateTime CreatedAt,
        string? DecisionNote);

    // داخل المدرس الحالي (الفلتر العام يعمل). الحفظ يتم عبر IUnitOfWork
    public interface IGroupAdminRepository
    {
        // عدد الطلاب المسؤولين النشطين في المجموعة
        Task<int> CountAdminsAsync(Guid groupId, CancellationToken cancellationToken);

        Task<List<GroupAdminItem>> GetAdminsAsync(Guid groupId, CancellationToken cancellationToken);

        void AddRequest(GroupAdminRequest request);
        Task<GroupAdminRequest?> GetTrackedRequestAsync(Guid requestId, CancellationToken cancellationToken);
        Task<bool> HasPendingRequestAsync(Guid groupId, Guid studentId, CancellationToken cancellationToken);

        Task<PagedResult<GroupAdminRequestItem>> GetRequestsPageAsync(
            GroupAdminRequestStatus? status, PageRequest page, CancellationToken cancellationToken);
    }
}
