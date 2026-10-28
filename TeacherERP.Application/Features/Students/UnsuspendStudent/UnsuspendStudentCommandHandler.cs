// جديد — TeacherERP.Application/Features/Students/UnsuspendStudent/UnsuspendStudentCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.UnsuspendStudent
{
    public class UnsuspendStudentCommandHandler : IRequestHandler<UnsuspendStudentCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnsuspendStudentCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IEnrollmentRepository enrollmentRepository,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _enrollmentRepository = enrollmentRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UnsuspendStudentCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } actorId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            var member = await _enrollmentRepository.GetMemberAsync(tenant.TenantId, request.StudentId, cancellationToken);
            if (member is null || member.Role != TenantRole.Student)
            {
                return Result<bool>.Failure(StudentErrors.NotFound);
            }

            if (member.Status != MembershipStatus.Suspended)
            {
                return Result<bool>.Failure(StudentErrors.NotSuspended);
            }

            // بداية جديدة: يرجع نشطاً والعداد صفر
            member.Activate();
            member.ResetAbsence();

            _auditRepository.Add(AuditLog.Create(tenant.TenantId, actorId, AuditAction.StudentUnsuspended, member.UserId, request.Reason));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}