// جديد — TeacherERP.Application/Features/Attendance/GetPendingCompensations/GetPendingCompensationsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetPendingCompensations
{
    public class GetPendingCompensationsQueryHandler : IRequestHandler<GetPendingCompensationsQuery, Result<List<PendingCompensationItem>>>
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

        public async Task<Result<List<PendingCompensationItem>>> Handle(GetPendingCompensationsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<PendingCompensationItem>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<List<PendingCompensationItem>>.Failure(TenantErrors.Required);
            }

            var items = await _attendanceRepository.GetPendingCompensationsAsync(userId, tenant.Role == TenantRole.Owner, cancellationToken);
            return Result<List<PendingCompensationItem>>.Success(items);
        }
    }
}