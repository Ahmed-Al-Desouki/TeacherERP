using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.MoveStudent;

public record MoveStudentCommand(Guid StudentId, Guid TargetGroupId) : IRequest<Result<bool>>;
