using MediatR;
using TeacherERP.Application.Features.Groups;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule.MoveSession;

public sealed class MoveSessionCommandHandler : IRequestHandler<MoveSessionCommand, Result<SessionTimeDto>>
{
    private readonly ICurrentTenantAccessor _currentTenant;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupSessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public MoveSessionCommandHandler(
        ICurrentTenantAccessor currentTenant,
        IGroupRepository groupRepository,
        IGroupSessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _currentTenant = currentTenant;
        _groupRepository = groupRepository;
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<SessionTimeDto>> Handle(MoveSessionCommand request, CancellationToken cancellationToken)
    {
        if (_currentTenant.Current is not { } tenant)
            return Result<SessionTimeDto>.Failure(TenantErrors.Required);

        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result<SessionTimeDto>.Failure(ScheduleErrors.NotFound);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        if (session.StartTime <= now)
            return Result<SessionTimeDto>.Failure(ScheduleErrors.InPast);
        if (session.IsCancelled)
            return Result<SessionTimeDto>.Failure(ScheduleErrors.Cancelled);

        var blockers = await _sessionRepository.GetMoveBlockersAsync(session.Id, cancellationToken);
        if (blockers.HasAttendance)
            return Result<SessionTimeDto>.Failure(ScheduleErrors.MoveBlockedByAttendance);
        if (blockers.HasCompensationReference)
            return Result<SessionTimeDto>.Failure(ScheduleErrors.MoveBlockedByCompensation);
        if (blockers.HasPendingWalkInRequest)
            return Result<SessionTimeDto>.Failure(ScheduleErrors.MoveBlockedByRequest);

        var targetGroup = await _groupRepository.GetWithSlotsAsync(request.TargetGroupId, cancellationToken);
        if (targetGroup is null)
            return Result<SessionTimeDto>.Failure(GroupErrors.NotFound);
        if (!targetGroup.IsActive)
            return Result<SessionTimeDto>.Failure(GroupErrors.Inactive);

        var newStart = SessionTimeCalculator.NormalizeUtc(request.NewStartTime);
        if (newStart <= now)
            return Result<SessionTimeDto>.Failure(ScheduleErrors.NewTimeInPast);

        var duration = request.DurationMinutes ?? (int)(session.EndTime - session.StartTime).TotalMinutes;
        var newEnd = newStart.AddMinutes(duration);

        if (await _sessionRepository.HasConflictAsync(
                targetGroup.Id, newStart, newEnd, new[] { session.Id }, cancellationToken))
            return Result<SessionTimeDto>.Failure(ScheduleErrors.TimeConflict);

        var conflicts = await _sessionRepository.GetCrossGroupConflictsAsync(
            tenant.TenantId, targetGroup.Id, targetGroup.CenterId, targetGroup.Room,
            newStart, newEnd, new[] { session.Id }, cancellationToken);
        if (conflicts.Any(x => x.TenantId == tenant.TenantId))
            return Result<SessionTimeDto>.Failure(ScheduleErrors.TimeConflict);

        var roomConflicts = conflicts.Where(x => x.TenantId != tenant.TenantId).ToArray();
        if (roomConflicts.Length > 0 && !request.ConfirmRoomConflicts)
            return Result<SessionTimeDto>.Failure(ScheduleErrors.RoomConflictConfirmationRequired, roomConflicts);

        session.MoveToGroup(targetGroup.Id, newStart, newEnd);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SessionTimeDto>.Success(SessionTimeDto.From(session));
    }
}
