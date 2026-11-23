using MediatR;
using TeacherERP.Application.Features.Enrollment;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Features.Enrollment.RequestGroup;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.RequestGroupByCode;

public sealed class RequestGroupByCodeCommandHandler
    : IRequestHandler<RequestGroupByCodeCommand, Result<EnrollmentRequestDto>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupJoinCodeService _codeService;
    private readonly ISender _sender;
    private readonly TimeProvider _timeProvider;

    public RequestGroupByCodeCommandHandler(
        IGroupRepository groupRepository,
        IGroupJoinCodeService codeService,
        ISender sender,
        TimeProvider timeProvider)
    {
        _groupRepository = groupRepository;
        _codeService = codeService;
        _sender = sender;
        _timeProvider = timeProvider;
    }

    public async Task<Result<EnrollmentRequestDto>> Handle(
        RequestGroupByCodeCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();
        GroupJoinCodeLookup? lookup;

        if (code.StartsWith("D.", StringComparison.Ordinal))
        {
            if (!_codeService.TryGetDynamicGroupId(code, out var groupId))
                return InvalidCode();

            lookup = await _groupRepository.GetJoinCodeLookupAsync(groupId, cancellationToken);
            if (lookup is null
                || !lookup.DynamicJoinEnabled
                || string.IsNullOrWhiteSpace(lookup.DynamicJoinSecret)
                || !_codeService.IsValidDynamicCode(
                    lookup.GroupId, lookup.DynamicJoinSecret, code, _timeProvider.GetUtcNow().UtcDateTime))
            {
                return InvalidCode();
            }
        }
        else
        {
            var normalizedCode = _codeService.NormalizeStaticCode(code);
            if (normalizedCode.Length is < 8 or > 20)
                return InvalidCode();

            lookup = await _groupRepository.GetJoinCodeLookupByStaticCodeAsync(normalizedCode, cancellationToken);
            if (lookup is null || !lookup.StaticJoinEnabled)
                return InvalidCode();
        }

        if (!lookup.GroupIsActive || lookup.TenantStatus != TenantStatus.Active)
            return InvalidCode();

        // تمرير الطلب إلى المسار المعتاد يحافظ على قواعد السعة والحظر والنقل والموافقة التلقائية.
        return await _sender.Send(new RequestGroupCommand(lookup.GroupId), cancellationToken);
    }

    private static Result<EnrollmentRequestDto> InvalidCode()
        => Result<EnrollmentRequestDto>.Failure(EnrollmentErrors.GroupNotFound);
}
