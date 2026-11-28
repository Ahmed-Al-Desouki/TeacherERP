namespace TeacherERP.Application.Features.Schedule.MoveSession;

public record MoveSessionRequest(
    Guid TargetGroupId,
    DateTime NewStartTime,
    int? DurationMinutes,
    bool ConfirmRoomConflicts = false);
