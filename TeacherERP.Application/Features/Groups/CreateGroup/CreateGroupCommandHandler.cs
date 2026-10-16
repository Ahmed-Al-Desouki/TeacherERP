using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.CreateGroup
{
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, Result<GroupDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ICenterRepository _centerRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionGenerator _sessionGenerator;

        public CreateGroupCommandHandler(
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

        public async Task<Result<GroupDto>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            // السنتر الحالي ثبّته الـ Policy (TenantOwner) بعد التحقق من العضوية والدور
            if (_currentTenant.Current is not { } tenant)
            {
                return Result<GroupDto>.Failure(TenantErrors.Required);
            }

            // السنتر لازم يكون مكاناً يدرّس فيه هذا المدرس فعلاً (نشط)
            if (!await _centerRepository.IsActiveTeacherAtAsync(request.CenterId, tenant.TenantId, cancellationToken))
            {
                return Result<GroupDto>.Failure(GroupErrors.CenterNotAllowed);
            }

            if (await _groupRepository.NameExistsAsync(request.Name.Trim(), cancellationToken))
            {
                return Result<GroupDto>.Failure(GroupErrors.NameTaken);
            }

            var group = Group.Create(
                tenant.TenantId, request.Name, request.GradeLevel, request.CenterId,
                request.Room, request.Capacity, request.WhatsAppGroupLink);

            foreach (var slot in request.Slots)
            {
                group.AddSlot(slot.Day, slot.StartTime, slot.DurationMinutes);
            }

            _groupRepository.Add(group);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // الحصص القادمة تُولَّد فوراً، فيظهر الجدول للمدرس والطلاب بدون انتظار المهمة اليومية
            await _sessionGenerator.GenerateForGroupAsync(group.Id, cancellationToken);

            return Result<GroupDto>.Success(GroupDto.From(group));
        }
    }
}