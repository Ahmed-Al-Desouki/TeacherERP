using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetLearningTimeline;

public sealed class GetStudentLearningTimelineQueryHandler
    : IRequestHandler<GetStudentLearningTimelineQuery, Result<PagedResult<StudentTimelineSessionDto>>>
{
    private static readonly TimeSpan MaximumRange = TimeSpan.FromDays(366);
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantAccessor _currentTenant;
    private readonly IStudentTimelineRepository _timelineRepository;
    private readonly ISessionAttachmentRepository _attachmentRepository;
    private readonly TimeProvider _timeProvider;

    public GetStudentLearningTimelineQueryHandler(ICurrentUserService currentUser,
        ICurrentTenantAccessor currentTenant, IStudentTimelineRepository timelineRepository,
        ISessionAttachmentRepository attachmentRepository, TimeProvider timeProvider)
    {
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _timelineRepository = timelineRepository;
        _attachmentRepository = attachmentRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Result<PagedResult<StudentTimelineSessionDto>>> Handle(
        GetStudentLearningTimelineQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } studentId)
            return Result<PagedResult<StudentTimelineSessionDto>>.Failure(
                new Error("Auth.Unauthorized", "You must be logged in."));

        if (_currentTenant.Current is not { } tenant)
            return Result<PagedResult<StudentTimelineSessionDto>>.Failure(TenantErrors.Required);

        if (tenant.Role != TenantRole.Student)
            return Result<PagedResult<StudentTimelineSessionDto>>.Failure(
                new Error("StudentTimeline.StudentOnly", "This timeline is available to students only."));

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var fromUtc = request.From is { } from
            ? SessionTimeCalculator.NormalizeUtc(from)
            : now.AddDays(-30);
        var toUtc = request.To is { } to
            ? SessionTimeCalculator.NormalizeUtc(to)
            : now.AddDays(90);

        if (toUtc <= fromUtc || toUtc - fromUtc > MaximumRange || request.GroupId == Guid.Empty)
            return Result<PagedResult<StudentTimelineSessionDto>>.Failure(
                new Error("StudentTimeline.InvalidRange", "The timeline range or group is invalid."));

        var page = await _timelineRepository.GetPageAsync(tenant.TenantId, studentId,
            fromUtc, toUtc, request.GroupId, PageRequest.From(request), cancellationToken);
        var sessionIds = page.Items.Select(item => item.SessionId).ToArray();
        var attachments = await _attachmentRepository.GetForSessionsAsync(
            tenant.TenantId, sessionIds, cancellationToken);
        var attachmentsBySession = attachments.GroupBy(item => item.SessionId)
            .ToDictionary(group => group.Key,
                group => (IReadOnlyList<StudentTimelineAttachmentDto>)group
                    .Select(StudentTimelineAttachmentDto.From).ToList());

        var result = page.Map(session => StudentTimelineSessionDto.From(session,
            attachmentsBySession.GetValueOrDefault(session.SessionId)
                ?? Array.Empty<StudentTimelineAttachmentDto>()));
        return Result<PagedResult<StudentTimelineSessionDto>>.Success(result);
    }
}
