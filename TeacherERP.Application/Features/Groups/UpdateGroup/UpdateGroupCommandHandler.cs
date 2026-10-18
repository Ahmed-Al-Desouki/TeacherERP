// جديد — TeacherERP.Application/Features/Groups/UpdateGroup/UpdateGroupCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.UpdateGroup
{
    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, Result<GroupDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ICenterRepository _centerRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionGenerator _sessionGenerator;

        public UpdateGroupCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ICenterRepository centerRepository,
            IGroupRepository groupRepository,
            IUnitOfWork unitOfWork,
            ISessionGenerator sessionGenerator)
        {
            _currentTenant = currentTenant;
            _centerRepository = centerRepository;
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
            _sessionGenerator = sessionGenerator;
        }

        public async Task<Result<GroupDto>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<GroupDto>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن المجموعة تخص المدرس الحالي فقط
            var group = await _groupRepository.GetTrackedWithSlotsAsync(request.GroupId, cancellationToken);
            if (group is null)
            {
                return Result<GroupDto>.Failure(GroupErrors.NotFound);
            }

            if (!await _centerRepository.IsActiveTeacherAtAsync(request.CenterId, tenant.TenantId, cancellationToken))
            {
                return Result<GroupDto>.Failure(GroupErrors.CenterNotAllowed);
            }

            if (await _groupRepository.NameExistsExceptAsync(request.Name.Trim(), group.Id, cancellationToken))
            {
                return Result<GroupDto>.Failure(GroupErrors.NameTaken);
            }

            group.UpdateDetails(request.Name, request.GradeLevel, request.CenterId, request.Room, request.Capacity, request.WhatsAppGroupLink);
            group.SetActive(request.IsActive);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // إعادة تفعيل مجموعة مؤرشفة تستكمل حصصها القادمة (الأرشفة تكتفي بإيقاف التوليد)
            if (group.IsActive)
            {
                await _sessionGenerator.GenerateForGroupAsync(group.Id, cancellationToken);
            }

            return Result<GroupDto>.Success(GroupDto.From(group));
        }
    }
}