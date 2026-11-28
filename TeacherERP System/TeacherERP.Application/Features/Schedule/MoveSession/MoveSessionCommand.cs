using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.MoveSession;

public record MoveSessionCommand(
    Guid SessionId,
    Guid TargetGroupId,
    DateTime NewStartTime,
    int? DurationMinutes,
    bool ConfirmRoomConflicts = false) : IRequest<Result<SessionTimeDto>>;
