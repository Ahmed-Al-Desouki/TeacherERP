using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.GetMySessions
{
    public record GetMySessionsQuery : IRequest<Result<List<SessionDto>>>;
}