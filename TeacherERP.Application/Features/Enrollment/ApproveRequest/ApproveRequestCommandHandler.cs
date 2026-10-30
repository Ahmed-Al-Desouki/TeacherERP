// موجود (تعديل) — TeacherERP.Application/Features/Enrollment/ApproveRequest/ApproveRequestCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.ApproveRequest
{
    public class ApproveRequestCommandHandler : IRequestHandler<ApproveRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IEnrollmentRepository _repository;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApproveRequestCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IEnrollmentRepository repository,
            IEnrollmentService enrollmentService,
            IGroupSessionRepository sessionRepository,
            IAttendanceRepository attendanceRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _repository = repository;
            _enrollmentService = enrollmentService;
            _sessionRepository = sessionRepository;
            _attendanceRepository = attendanceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(ApproveRequestCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن الطلب يخص المدرس الحالي فقط
            var enrollmentRequest = await _repository.GetTrackedRequestAsync(request.RequestId, cancellationToken);
            if (enrollmentRequest is null)
            {
                return Result<bool>.Failure(EnrollmentErrors.RequestNotFound);
            }

            if (enrollmentRequest.Status != EnrollmentRequestStatus.Pending)
            {
                return Result<bool>.Failure(EnrollmentErrors.NotPending);
            }

            var approved = await _enrollmentService.ApproveAsync(enrollmentRequest, userId, cancellationToken);
            if (approved.IsFailure)
            {
                return approved;
            }

            // طالب مسح الكود في حصة وهو غير مسجّل (Walk-in) وطلبه كان معلّقاً: الموافقة تسجل له حضور الحصة اللي حضرها فعلاً
            if (enrollmentRequest.SourceSessionId is { } sessionId)
            {
                await RecordWalkInAttendanceAsync(tenant.TenantId, enrollmentRequest.StudentId, sessionId, userId, cancellationToken);
            }

            return approved;
        }

        private async Task RecordWalkInAttendanceAsync(
            Guid tenantId, Guid studentId, Guid sessionId, Guid approvedByUserId, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
            if (session is null || session.IsCancelled)
            {
                return;
            }

            // لو في سجل بالفعل (مسح تاني مثلاً) ما نكررش
            if (await _attendanceRepository.GetRecordAsync(sessionId, studentId, cancellationToken) is not null)
            {
                return;
            }

            // الطالب مسح بنفسه، والموافِق هو المسؤول عن تسجيله
            _attendanceRepository.AddRecord(AttendanceRecord.RecordPresence(
                tenantId, sessionId, studentId, ScanMethod.SelfScan, null, null, approvedByUserId));

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}