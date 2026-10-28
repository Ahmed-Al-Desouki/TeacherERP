// موجود (تعديل) — TeacherERP.Application/Features/Attendance/RejectCompensation/RejectCompensationCommandHandler.cs
using System.Data;
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
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IAbsenceRepository _absenceRepository;
        private readonly IAbsenceService _absenceService;
        private readonly IUnitOfWork _unitOfWork;

        public RejectCompensationCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IAttendanceRepository attendanceRepository,
            IEnrollmentRepository enrollmentRepository,
            IGroupSessionRepository sessionRepository,
            IAbsenceRepository absenceRepository,
            IAbsenceService absenceService,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _attendanceRepository = attendanceRepository;
            _enrollmentRepository = enrollmentRepository;
            _sessionRepository = sessionRepository;
            _absenceRepository = absenceRepository;
            _absenceService = absenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(RejectCompensationCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { })
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

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                record.RejectCompensation();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // الحصة الأصلية اللي كان التعويض يغطيها ممكن تكون اتعالجت بدون غياب (لأنه كان معلّقاً): الغياب يُحسب الآن.
                // لو لسه ما اتعالجتش، الـ Job هيحسبها لوحده لأن الرفض لا يغطي الحصة
                if (record.CompensatesSessionId is { } targetId)
                {
                    var target = await _sessionRepository.GetByIdAsync(targetId, cancellationToken);

                    if (target is { IsCancelled: false, AbsencesProcessedAt: not null })
                    {
                        // يتحقق من التسجيل الساري وقتها وعدم وجود حضور فعلي، بعد حفظ حالة الرفض فوق
                        var absentIds = await _absenceRepository.GetAbsentCandidateIdsAsync(target, cancellationToken);

                        if (absentIds.Contains(record.StudentId))
                        {
                            var member = await _enrollmentRepository.GetMemberAsync(tenant.TenantId, record.StudentId, cancellationToken);

                            if (member is { Status: MembershipStatus.Active })
                            {
                                await _absenceService.RegisterAbsenceAsync(member, targetId, cancellationToken);
                                await _unitOfWork.SaveChangesAsync(cancellationToken);
                            }
                        }
                    }
                }

                return Result<bool>.Success(true);
            }, IsolationLevel.ReadCommitted, cancellationToken);
        }
    }
}