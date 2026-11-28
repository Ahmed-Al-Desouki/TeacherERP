// جديد — TeacherERP.Application/Features/GroupAdmins/GetGroupSessionAttendance/GetGroupSessionAttendanceQueryHandler.cs
using MediatR;
using TeacherERP.Application.Attendance;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Attendance;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GetGroupSessionAttendance
{
    public class GetGroupSessionAttendanceQueryHandler
        : IRequestHandler<GetGroupSessionAttendanceQuery, Result<PagedResult<AttendanceListItem>>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupAccessService _groupAccess;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly TimeProvider _timeProvider;

        public GetGroupSessionAttendanceQueryHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupAccessService groupAccess,
            IAttendanceRepository attendanceRepository,
            TimeProvider timeProvider)
        {
            _currentTenant = currentTenant;
            _groupAccess = groupAccess;
            _attendanceRepository = attendanceRepository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<PagedResult<AttendanceListItem>>> Handle(
            GetGroupSessionAttendanceQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<PagedResult<AttendanceListItem>>.Failure(TenantErrors.Required);
            }

            var group = await _groupAccess.GetAdminGroupIdAsync(cancellationToken);
            if (group.IsFailure)
            {
                return Result<PagedResult<AttendanceListItem>>.Failure(group.Error);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var running = await _attendanceRepository.GetRunningSessionsAsync(
                tenant.TenantId, now, AttendancePolicy.EarlyCheckInMinutes, group.Value, cancellationToken);

            if (running.Count == 0)
            {
                return Result<PagedResult<AttendanceListItem>>.Failure(AttendanceErrors.NoSessionNow);
            }

            var page = await _attendanceRepository.GetSessionAttendancePageAsync(
                running[0].SessionId, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<AttendanceListItem>>.Success(page);
        }
    }
}