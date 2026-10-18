// جديد — TeacherERP.Application/Enrollment/EnrollmentService.cs
using System.Data;
using TeacherERP.Application.Features.Enrollment;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Enrollment
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public EnrollmentService(IEnrollmentRepository repository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public Task<Result<bool>> ApproveAsync(EnrollmentRequest request, Guid? decidedByUserId, CancellationToken cancellationToken)
        {
            // Serializable: فحص السعة والتسجيل معاً بدون أن يسبقنا طلب متزامن على آخر مقعد
            return _unitOfWork.ExecuteInTransactionAsync(
                () => ApproveCoreAsync(request, decidedByUserId, cancellationToken),
                IsolationLevel.Serializable,
                cancellationToken);
        }

        private async Task<Result<bool>> ApproveCoreAsync(EnrollmentRequest request, Guid? decidedByUserId, CancellationToken cancellationToken)
        {
            var group = await _repository.GetGroupJoinInfoAsync(request.GroupId, cancellationToken);
            if (group is null || group.TenantId != request.TenantId || !group.GroupIsActive || group.TenantStatus != TenantStatus.Active)
            {
                return Result<bool>.Failure(EnrollmentErrors.GroupNotFound);
            }

            var member = await _repository.GetMemberAsync(request.TenantId, request.StudentId, cancellationToken);
            if (member is { Status: MembershipStatus.Blacklisted })
            {
                return Result<bool>.Failure(EnrollmentErrors.NotAllowed);
            }

            var active = await _repository.GetActiveEnrollmentAsync(request.TenantId, request.StudentId, cancellationToken);

            if (request.Type == EnrollmentRequestType.Join)
            {
                if (active is not null)
                {
                    return Result<bool>.Failure(EnrollmentErrors.AlreadyEnrolled);
                }
            }
            else
            {
                if (active is null)
                {
                    return Result<bool>.Failure(EnrollmentErrors.NotEnrolled);
                }

                if (active.GroupId == request.GroupId)
                {
                    return Result<bool>.Failure(EnrollmentErrors.SameGroup);
                }
            }

            if (group.Capacity is { } capacity
                && await _repository.CountActiveAsync(request.TenantId, request.GroupId, cancellationToken) >= capacity)
            {
                return Result<bool>.Failure(EnrollmentErrors.GroupFull);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            // أول انضمام عند هذا المدرس يُنشئ عضوية الطالب (الغياب والمديونية تعيش عليها وتنتقل مع الطالب بين المجموعات)
            if (member is null)
            {
                _repository.AddMember(TenantMember.Create(request.TenantId, request.StudentId, TenantRole.Student));
            }

            // النقل: ننهي القديم ونحفظ أولاً، حتى لا يتعارض الـ Index الفريد (تسجيل نشط واحد) مع الجديد
            if (active is not null)
            {
                active.End(now);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            _repository.AddEnrollment(GroupEnrollment.Create(request.TenantId, request.GroupId, request.StudentId, now, decidedByUserId));
            request.Approve(decidedByUserId, now);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}