// جديد — TeacherERP.Application/Features/Attendance/GetDisplayCode/GetDisplayCodeQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetDisplayCode
{
    public class GetDisplayCodeQueryHandler : IRequestHandler<GetDisplayCodeQuery, Result<DisplayCodeDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IAttendanceCodeService _codeService;
        private readonly TimeProvider _timeProvider;

        public GetDisplayCodeQueryHandler(
            ICurrentTenantAccessor currentTenant,
            IAttendanceRepository attendanceRepository,
            IAttendanceCodeService codeService,
            TimeProvider timeProvider)
        {
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
            _codeService = codeService;
            _timeProvider = timeProvider;
        }

        public async Task<Result<DisplayCodeDto>> Handle(GetDisplayCodeQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<DisplayCodeDto>.Failure(TenantErrors.Required);
            }

            var settings = await _attendanceRepository.GetSettingsAsync(tenant.TenantId, cancellationToken);
            if (settings is null || settings.Mode != AttendanceMode.DynamicScreenQR)
            {
                return Result<DisplayCodeDto>.Failure(AttendanceErrors.NotDynamicMode);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            return Result<DisplayCodeDto>.Success(new DisplayCodeDto(
                tenant.TenantId,
                _codeService.GenerateDynamicCode(tenant.TenantId, settings.QrSecret, now),
                _codeService.SecondsRemaining(now)));
        }
    }
}