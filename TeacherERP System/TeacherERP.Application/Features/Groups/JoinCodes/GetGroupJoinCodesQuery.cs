using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.JoinCodes;

public sealed record GetGroupJoinCodesQuery(Guid GroupId) : IRequest<Result<GroupJoinCodesDto>>;
