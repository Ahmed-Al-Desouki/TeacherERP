// جديد — TeacherERP.Application/Features/Enrollment/RequestGroup/RequestGroupCommandHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.RequestGroup
{
    public class RequestGroupCommandHandler : IRequestHandler<RequestGroupCommand, Result<EnrollmentRequestDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IEnrollmentRepository _repository;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUnitOfWork _unitOfWork;

        public RequestGroupCommandHandler(
            ICurrentUserService currentUser,
            IEnrollmentRepository repository,
            IEnrollmentService enrollmentService,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _repository = repository;
            _enrollmentService = enrollmentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<EnrollmentRequestDto>> Handle(RequestGroupCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } studentId)
            {
                return Result<EnrollmentRequestDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            // الطالب ليس عضواً بعد، فالبحث عن المجموعة عبر المدرسين بقصد (والـ Id غير قابل للتخمين)
            var group = await _repository.GetGroupJoinInfoAsync(request.GroupId, cancellationToken);
            if (group is null || !group.GroupIsActive || group.TenantStatus != TenantStatus.Active)
            {
                return Result<EnrollmentRequestDto>.Failure(EnrollmentErrors.GroupNotFound);
            }

            var member = await _repository.GetMemberAsync(group.TenantId, studentId, cancellationToken);
            if (member is { Status: MembershipStatus.Blacklisted })
            {
                return Result<EnrollmentRequestDto>.Failure(EnrollmentErrors.NotAllowed);
            }

            var active = await _repository.GetActiveEnrollmentAsync(group.TenantId, studentId, cancellationToken);
            if (active is not null && active.GroupId == group.GroupId)
            {
                return Result<EnrollmentRequestDto>.Failure(EnrollmentErrors.SameGroup);
            }

            if (await _repository.HasPendingRequestAsync(group.TenantId, studentId, cancellationToken))
            {
                return Result<EnrollmentRequestDto>.Failure(EnrollmentErrors.AlreadyPending);
            }

            if (group.Capacity is { } capacity
                && await _repository.CountActiveAsync(group.TenantId, group.GroupId, cancellationToken) >= capacity)
            {
                return Result<EnrollmentRequestDto>.Failure(EnrollmentErrors.GroupFull);
            }

            var type = active is null ? EnrollmentRequestType.Join : EnrollmentRequestType.Transfer;
            var enrollmentRequest = EnrollmentRequest.Create(group.TenantId, studentId, group.GroupId, type);
            _repository.AddRequest(enrollmentRequest);

            // القبول التلقائي للانضمام فقط (إعداد المدرس)، أما النقل بين المجموعات فيحتاج موافقة دائماً
            if (type == EnrollmentRequestType.Join && group.AutoApprove)
            {
                var approved = await _enrollmentService.ApproveAsync(enrollmentRequest, null, cancellationToken);
                if (approved.IsFailure)
                {
                    return Result<EnrollmentRequestDto>.Failure(approved.Error);
                }
            }
            else
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<EnrollmentRequestDto>.Success(new EnrollmentRequestDto(
                enrollmentRequest.Id, group.GroupId, group.GroupName, group.TenantId,
                enrollmentRequest.Type, enrollmentRequest.Status, enrollmentRequest.CreatedAt, null));
        }
    }
}