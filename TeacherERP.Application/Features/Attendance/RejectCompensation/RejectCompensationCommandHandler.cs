// موجود (تعديل) — TeacherERP.Application/Features/Attendance/RejectCompensation/RejectCompensationCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.RejectCompensation
{
    public class RejectCompensationCommandHandler : IRequestHandler<RejectCompensationCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RejectCompensationCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IAttendanceRepository attendanceRepository,
            IEnrollmentRepository enrollmentRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(RejectCompensationCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن السجل يخص المدرس الحالي فقط
            var record = await _attendanceRepository.GetTrackedRecordAsync(request.RecordId, cancellationToken);
            if (record is null)
            {
                return Result<bool>.Failure(AttendanceErrors.RecordNotFound);
            }

            if (record.Status != AttendanceStatus.PendingCompensation)
            {
                return Result<bool>.Failure(AttendanceErrors.NotPending);
            }

            record.RejectCompensation();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}