// موجود (تعديل) — TeacherERP.Application/Features/Attendance/GetPendingCompensations/GetPendingCompensationsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetPendingCompensations
{
    public class GetPendingCompensationsQueryHandler
        : IRequestHandler<GetPendingCompensationsQuery, Result<PagedResult<PendingCompensationItem>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAttendanceRepository _attendanceRepository;

        public GetPendingCompensationsQueryHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IAttendanceRepository attendanceRepository)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<Result<PagedResult<PendingCompensationItem>>> Handle(
            GetPendingCompensationsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { })
            {
                return Result<PagedResult<PendingCompensationItem>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is null)
            {
                return Result<PagedResult<PendingCompensationItem>>.Failure(TenantErrors.Required);
            }

            var page = await _attendanceRepository.GetPendingCompensationsPageAsync(PageRequest.From(request), cancellationToken);

            return Result<PagedResult<PendingCompensationItem>>.Success(page);
        }
    }
}
