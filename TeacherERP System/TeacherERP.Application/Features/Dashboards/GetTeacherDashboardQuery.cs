using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Dashboards;

public sealed record GetTeacherDashboardQuery : IRequest<Result<TeacherDashboardDto>>;
