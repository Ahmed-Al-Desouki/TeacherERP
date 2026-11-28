using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.JoinCodes;

public sealed record UpdateGroupJoinCodesCommand(
    Guid GroupId,
    bool StaticEnabled,
    bool DynamicEnabled,
    bool RotateStaticCode = false,
    bool RotateDynamicCode = false) : IRequest<Result<GroupJoinCodesDto>>;
