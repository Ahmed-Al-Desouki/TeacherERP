// جديد — TeacherERP.Application/Features/Team/GetTeam/GetTeamQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.GetTeam
{
    // مساعدو المدرس الحالي (للمدرس فقط عبر الـ Policy)
    public record GetTeamQuery : IRequest<Result<List<TeamMemberDto>>>;
}