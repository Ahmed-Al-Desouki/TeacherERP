// جديد — TeacherERP.Application/Features/GroupAdmins/ApproveGroupAdminRequest/ApproveGroupAdminRequestCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.ApproveGroupAdminRequest
{
    public class ApproveGroupAdminRequestCommandHandler : IRequestHandler<ApproveGroupAdminRequestCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupAdminRepository _groupAdminRepository;
        private readonly IGroupAdminService _groupAdminService;

        public ApproveGroupAdminRequestCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IGroupAdminRepository groupAdminRepository,
            IGroupAdminService groupAdminService)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _groupAdminRepository = groupAdminRepository;
            _groupAdminService = groupAdminService;
        }

        public async Task<Result<bool>> Handle(ApproveGroupAdminRequestCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is null)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن الطلب يخص المدرس الحالي فقط
            var pending = await _groupAdminRepository.GetTrackedRequestAsync(request.RequestId, cancellationToken);
            if (pending is null)
            {
                return Result<bool>.Failure(GroupAdminErrors.RequestNotFound);
            }

            if (pending.Status != GroupAdminRequestStatus.Pending)
            {
                return Result<bool>.Failure(GroupAdminErrors.RequestNotPending);
            }

            // فشل التنفيذ (الطالب اتنقل مثلاً أو الحد اكتمل) يبقي الطلب معلّقاً، والمدرس يقدر يرفضه
            return await _groupAdminService.ApplyAsync(
                pending.Type,
                pending.GroupId,
                pending.StudentId,
                userId,
                $"Approved the assistant's request {pending.Id}",
                pending,
                cancellationToken);
        }
    }
}