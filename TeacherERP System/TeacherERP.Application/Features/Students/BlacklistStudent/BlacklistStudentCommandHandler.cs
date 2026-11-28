// جديد — TeacherERP.Application/Features/Students/BlacklistStudent/BlacklistStudentCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.BlacklistStudent
{
    public class BlacklistStudentCommandHandler : IRequestHandler<BlacklistStudentCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public BlacklistStudentCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IEnrollmentRepository enrollmentRepository,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _enrollmentRepository = enrollmentRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(BlacklistStudentCommand request, CancellationToken cancellationToken)
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

            if (member.Status == MembershipStatus.Blacklisted)
            {
                return Result<bool>.Failure(StudentErrors.AlreadyBlacklisted);
            }

            member.Blacklist();

            // لا غياب ولا مقعد في المجموعة بعد الحظر (طلب انضمام معلّق قديم يفشل عند الموافقة لأن الطالب محظور)
            var active = await _enrollmentRepository.GetActiveEnrollmentAsync(tenant.TenantId, member.UserId, cancellationToken);
            active?.End(_timeProvider.GetUtcNow().UtcDateTime);

            _auditRepository.Add(AuditLog.Create(tenant.TenantId, actorId, AuditAction.StudentBlacklisted, member.UserId, request.Reason));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}