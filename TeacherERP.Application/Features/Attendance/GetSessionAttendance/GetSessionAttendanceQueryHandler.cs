// موجود (تعديل) — TeacherERP.Application/Features/Attendance/GetSessionAttendance/GetSessionAttendanceQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetSessionAttendance
{
    public class GetSessionAttendanceQueryHandler : IRequestHandler<GetSessionAttendanceQuery, Result<PagedResult<AttendanceListItem>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAttendanceRepository _attendanceRepository;

        public GetSessionAttendanceQueryHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IAttendanceRepository attendanceRepository)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<Result<PagedResult<AttendanceListItem>>> Handle(
            GetSessionAttendanceQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { })
            {
                return Result<PagedResult<AttendanceListItem>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is null)
            {
                return Result<PagedResult<AttendanceListItem>>.Failure(TenantErrors.Required);
            }

            var page = await _attendanceRepository.GetSessionAttendancePageAsync(
                request.SessionId, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<AttendanceListItem>>.Success(page);
        }
    }
}
