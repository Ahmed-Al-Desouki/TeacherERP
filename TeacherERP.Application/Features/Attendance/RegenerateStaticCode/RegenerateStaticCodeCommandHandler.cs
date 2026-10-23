// جديد — TeacherERP.Application/Features/Attendance/RegenerateStaticCode/RegenerateStaticCodeCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.RegenerateStaticCode
{
    public class RegenerateStaticCodeCommandHandler : IRequestHandler<RegenerateStaticCodeCommand, Result<AttendanceSettingsDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegenerateStaticCodeCommandHandler(
            ICurrentTenantAccessor currentTenant,
            IAttendanceRepository attendanceRepository,
            IUnitOfWork unitOfWork)
        {
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AttendanceSettingsDto>> Handle(RegenerateStaticCodeCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<AttendanceSettingsDto>.Failure(TenantErrors.Required);
            }

            var settings = await _attendanceRepository.GetTrackedSettingsAsync(cancellationToken);
            if (settings is null)
            {
                return Result<AttendanceSettingsDto>.Failure(TenantErrors.Forbidden);
            }

            settings.RegenerateQrSecret();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var staticCode = settings.Mode == AttendanceMode.StaticGlobalQR ? settings.QrSecret : null;

            return Result<AttendanceSettingsDto>.Success(new AttendanceSettingsDto(
                tenant.TenantId, settings.Mode, settings.RejectMockLocations, settings.AutoRegisterOnScan, staticCode));
        }
    }
}