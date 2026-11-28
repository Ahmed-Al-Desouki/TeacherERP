using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetWeeklySchedule;

public sealed record GetStudentWeeklyScheduleQuery : IRequest<Result<StudentWeeklyScheduleDto>>;
