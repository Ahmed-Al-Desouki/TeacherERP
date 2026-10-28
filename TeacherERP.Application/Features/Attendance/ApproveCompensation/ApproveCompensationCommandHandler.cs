// موجود (تعديل) — TeacherERP.Application/Features/Attendance/ApproveCompensation/ApproveCompensationCommandHandler.cs
using MediatR;
using TeacherERP.Application.Absence;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.ApproveCompensation
{
    public class ApproveCompensationCommandHandler : IRequestHandler<ApproveCompensationCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveCompensationCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IAttendanceRepository attendanceRepository,
            IEnrollmentRepository enrollmentRepository,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
            _enrollmentRepository = enrollmentRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(ApproveCompensationCommand request, CancellationToken cancellationToken)
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

            record.ApproveCompensation();

            var member = await _enrollmentRepository.GetMemberAsync(tenant.TenantId, record.StudentId, cancellationToken);
            if (member is not null)
            {
                // موقوف بسبب الغياب المتتالي؟ (العداد وصل الحد). نفحصها قبل تصفير العداد
                var suspendedByAbsence = member.Status == MembershipStatus.Suspended
                                         && member.ConsecutiveAbsences >= AbsencePolicy.AutoSuspendAfterConsecutiveAbsences;

                // الحضور (ولو تعويضاً) يصفّر عداد الغياب المتتالي عند هذا المدرس
                if (member.ConsecutiveAbsences > 0)
                {
                    member.ResetAbsence();
                }

                // التعويض المعتمد يرفع الإيقاف التلقائي (الشرط اللي أوقفه لم يعد قائماً)
                if (suspendedByAbsence)
                {
                    member.Activate();

                    _auditRepository.Add(AuditLog.Create(
                        tenant.TenantId, userId, AuditAction.StudentReinstatedByCompensation, member.UserId, "Compensation session approved"));
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}