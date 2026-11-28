// جديد — TeacherERP.Application/Features/GroupAdmins/RequestGroupAdminChange/RequestGroupAdminChangeCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Groups;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.RequestGroupAdminChange
{
    public class RequestGroupAdminChangeCommandHandler
        : IRequestHandler<RequestGroupAdminChangeCommand, Result<GroupAdminChangeResultDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IGroupAdminRepository _groupAdminRepository;
        private readonly IGroupAdminService _groupAdminService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RequestGroupAdminChangeCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            ITenantRepository tenantRepository,
            IGroupRepository groupRepository,
            IEnrollmentRepository enrollmentRepository,
            IGroupAdminRepository groupAdminRepository,
            IGroupAdminService groupAdminService,
            INotificationService notificationService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
            _groupRepository = groupRepository;
            _enrollmentRepository = enrollmentRepository;
            _groupAdminRepository = groupAdminRepository;
            _groupAdminService = groupAdminService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GroupAdminChangeResultDto>> Handle(
            RequestGroupAdminChangeCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<GroupAdminChangeResultDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } current)
            {
                return Result<GroupAdminChangeResultDto>.Failure(TenantErrors.Required);
            }

            var tenant = await _tenantRepository.GetByIdAsync(current.TenantId, cancellationToken);
            if (tenant is null)
            {
                return Result<GroupAdminChangeResultDto>.Failure(TenantErrors.Forbidden);
            }

            // المدرس، أو المساعد لما يكون المدرس فعّل الموافقة التلقائية: ينفَّذ فوراً
            var isOwner = current.Role == TenantRole.Owner;
            if (isOwner || tenant.AutoApproveGroupAdminChanges)
            {
                var reason = isOwner ? null : "Applied automatically (assistant request, auto-approve is on)";

                var applied = await _groupAdminService.ApplyAsync(
                    request.Type, request.GroupId, request.StudentId, userId, reason, null, cancellationToken);

                return applied.IsFailure
                    ? Result<GroupAdminChangeResultDto>.Failure(applied.Error)
                    : Result<GroupAdminChangeResultDto>.Success(new GroupAdminChangeResultDto(true, null));
            }

            return await CreatePendingRequestAsync(current.TenantId, userId, request, cancellationToken);
        }

        // المساعد: نتحقق من الأساسيات أولاً (فلا نخلق طلبات عشوائية) ثم نسجّل طلباً ينتظر المدرس
        private async Task<Result<GroupAdminChangeResultDto>> CreatePendingRequestAsync(
            Guid tenantId, Guid userId, RequestGroupAdminChangeCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetWithSlotsAsync(request.GroupId, cancellationToken);
            if (group is null)
            {
                return Result<GroupAdminChangeResultDto>.Failure(GroupErrors.NotFound);
            }

            if (!group.IsActive)
            {
                return Result<GroupAdminChangeResultDto>.Failure(GroupErrors.Inactive);
            }

            var enrollment = await _enrollmentRepository.GetActiveEnrollmentAsync(tenantId, request.StudentId, cancellationToken);
            if (enrollment is null || enrollment.GroupId != request.GroupId)
            {
                return Result<GroupAdminChangeResultDto>.Failure(GroupAdminErrors.NotEnrolled);
            }

            if (request.Type == GroupAdminRequestType.Appoint && enrollment.IsGroupAdmin)
            {
                return Result<GroupAdminChangeResultDto>.Failure(GroupAdminErrors.AlreadyAdmin);
            }

            if (request.Type == GroupAdminRequestType.Remove && !enrollment.IsGroupAdmin)
            {
                return Result<GroupAdminChangeResultDto>.Failure(GroupAdminErrors.NotAdmin);
            }

            if (await _groupAdminRepository.HasPendingRequestAsync(request.GroupId, request.StudentId, cancellationToken))
            {
                return Result<GroupAdminChangeResultDto>.Failure(GroupAdminErrors.RequestAlreadyPending);
            }

            var pending = GroupAdminRequest.Create(tenantId, request.GroupId, request.StudentId, request.Type, userId);
            _groupAdminRepository.AddRequest(pending);

            var requester = await _userRepository.GetByIdAsync(userId, cancellationToken);
            var student = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
            var verb = request.Type == GroupAdminRequestType.Appoint ? "appoint" : "remove";

            await _notificationService.NotifyStaffAsync(
                tenantId,
                NotificationType.GroupAdminChangeRequested,
                $"{requester?.FullName ?? "An assistant"} asked to {verb} {student?.FullName ?? "a student"} as a student admin of {group.Name}.",
                pending.Id,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<GroupAdminChangeResultDto>.Success(new GroupAdminChangeResultDto(false, pending.Id));
        }
    }
}