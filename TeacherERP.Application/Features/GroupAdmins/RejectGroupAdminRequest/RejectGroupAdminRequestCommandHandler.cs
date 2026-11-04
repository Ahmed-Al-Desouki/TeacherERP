// جديد — TeacherERP.Application/Features/GroupAdmins/RejectGroupAdminRequest/RejectGroupAdminRequestCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.RejectGroupAdminRequest
{
    public class RejectGroupAdminRequestCommandHandler : IRequestHandler<RejectGroupAdminRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupAdminRepository _groupAdminRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RejectGroupAdminRequestCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IGroupAdminRepository groupAdminRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _groupAdminRepository = groupAdminRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(RejectGroupAdminRequestCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is null)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            var pending = await _groupAdminRepository.GetTrackedRequestAsync(request.RequestId, cancellationToken);
            if (pending is null)
            {
                return Result<bool>.Failure(GroupAdminErrors.RequestNotFound);
            }

            if (pending.Status != GroupAdminRequestStatus.Pending)
            {
                return Result<bool>.Failure(GroupAdminErrors.RequestNotPending);
            }

            pending.Reject(userId, _timeProvider.GetUtcNow().UtcDateTime, request.Note);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}