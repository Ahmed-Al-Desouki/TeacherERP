// موجود (تعديل) — TeacherERP.Application/Features/Attendance/GetSessionAttendance/GetSessionAttendanceQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Schedule;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetSessionAttendance
{
    public class GetSessionAttendanceQueryHandler : IRequestHandler<GetSessionAttendanceQuery, Result<List<AttendanceListItem>>>
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

        public async Task<Result<List<AttendanceListItem>>> Handle(GetSessionAttendanceQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<AttendanceListItem>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<List<AttendanceListItem>>.Failure(TenantErrors.Required);
            }

            var items = await _attendanceRepository.GetSessionAttendanceAsync(request.SessionId, cancellationToken);
            return Result<List<AttendanceListItem>>.Success(items);
        }
    }
}