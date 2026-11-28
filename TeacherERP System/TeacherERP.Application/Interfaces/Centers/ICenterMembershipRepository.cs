using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Centers;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface ICenterMembershipRepository
    {
        Task<int> CountPendingRequestsAsync(Guid tenantId, CancellationToken cancellationToken);
        Task<CenterJoinRequest?> GetPendingRequestAsync(Guid centerId, Guid tenantId, CancellationToken cancellationToken);
        Task<CenterJoinRequest?> GetRequestForTeacherAsync(Guid requestId, Guid tenantId, CancellationToken cancellationToken);
        Task<CenterJoinRequest?> GetRequestForCenterOwnerAsync(Guid requestId, Guid ownerUserId, CancellationToken cancellationToken);
        Task<PagedResult<CenterJoinRequestListItem>> GetRequestsForOwnerAsync(
            Guid ownerUserId, CenterJoinRequestStatus? status, PageRequest page, CancellationToken cancellationToken);
        Task<PagedResult<CenterJoinRequestListItem>> GetRequestsForTeacherAsync(
            Guid tenantId, PageRequest page, CancellationToken cancellationToken);
        Task<CenterInvitation?> GetInvitationByHashAsync(string tokenHash, CancellationToken cancellationToken);
        void Add(CenterJoinRequest request);
        void Add(CenterInvitation invitation);
        Task<int> PurgeClosedDataBeforeAsync(DateTime cutoffUtc, int batchSize, CancellationToken cancellationToken);
    }
}
