using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.GetGroup
{
    public record GetGroupQuery(Guid GroupId) : IRequest<Result<GroupDto>>;
}