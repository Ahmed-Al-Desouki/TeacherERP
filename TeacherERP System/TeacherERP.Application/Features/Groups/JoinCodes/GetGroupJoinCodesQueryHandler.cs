using MediatR;
using TeacherERP.Application.Features.Groups;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.JoinCodes;

public sealed class GetGroupJoinCodesQueryHandler : IRequestHandler<GetGroupJoinCodesQuery, Result<GroupJoinCodesDto>>
{
    private readonly ICurrentTenantAccessor _currentTenant;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupJoinCodeService _codeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public GetGroupJoinCodesQueryHandler(
        ICurrentTenantAccessor currentTenant,
        IGroupRepository groupRepository,
        IGroupJoinCodeService codeService,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _currentTenant = currentTenant;
        _groupRepository = groupRepository;
        _codeService = codeService;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<GroupJoinCodesDto>> Handle(GetGroupJoinCodesQuery request, CancellationToken cancellationToken)
    {
        if (_currentTenant.Current is null)
            return Result<GroupJoinCodesDto>.Failure(TenantErrors.Required);

        var group = await _groupRepository.GetTrackedWithSlotsAsync(request.GroupId, cancellationToken);
        if (group is null)
            return Result<GroupJoinCodesDto>.Failure(GroupErrors.NotFound);

        if (group.EnsureJoinCodes())
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<GroupJoinCodesDto>.Success(GroupJoinCodesDto.From(
            group, _codeService, _timeProvider.GetUtcNow().UtcDateTime));
    }
}
