// جديد — TeacherERP.Application/Features/Attendance/GetAttendanceSettings/GetAttendanceSettingsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetAttendanceSettings
{
    public class GetAttendanceSettingsQueryHandler : IRequestHandler<GetAttendanceSettingsQuery, Result<AttendanceSettingsDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAttendanceRepository _attendanceRepository;

        public GetAttendanceSettingsQueryHandler(ICurrentTenantAccessor currentTenant, IAttendanceRepository attendanceRepository)
        {
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<Result<AttendanceSettingsDto>> Handle(GetAttendanceSettingsQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<AttendanceSettingsDto>.Failure(TenantErrors.Required);
            }

            var settings = await _attendanceRepository.GetSettingsAsync(tenant.TenantId, cancellationToken);
            if (settings is null)
            {
                return Result<AttendanceSettingsDto>.Failure(TenantErrors.Forbidden);
            }

            // السر لا يُعرض إلا في الوضع الثابت (للطباعة)، وفي المتغير لا حاجة لعرضه أبداً
            var staticCode = settings.Mode == AttendanceMode.StaticGlobalQR ? settings.QrSecret : null;

            return Result<AttendanceSettingsDto>.Success(new AttendanceSettingsDto(
                tenant.TenantId, settings.Mode, settings.RejectMockLocations, settings.AutoRegisterOnScan, staticCode));
        }
    }
}